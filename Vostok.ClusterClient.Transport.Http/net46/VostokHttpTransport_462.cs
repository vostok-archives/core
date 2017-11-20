using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Collections;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    // ReSharper disable MethodSupportsCancellation
    // ReSharper disable PossibleNullReferenceException

    public partial class VostokHttpTransport
    {
        private const int preferredReadSize = 16 * 1024;
        private const int lohObjectSizeThreshold = 85 * 1000;

        private static readonly TimeSpan requestAbortTimeout = TimeSpan.FromMilliseconds(250);
        private static readonly IPool<byte[]> readBuffersPool = new UnlimitedLazyPool<byte[]>(() => new byte[preferredReadSize]);

        private readonly ILog log;
        private readonly ConnectTimeLimiter connectTimeLimiter;
        private readonly ThreadPoolMonitor threadPoolMonitor;

        public VostokHttpTransport(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));

            connectTimeLimiter = new ConnectTimeLimiter(log);
            threadPoolMonitor = ThreadPoolMonitor.Instance;
        }

        public int ConnectionAttempts { get; set; } = 1;

        public TimeSpan? ConnectionTimeout { get; set; } = 2.Seconds();

        public async Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout.TotalMilliseconds < 1)
            {
                LogRequestTimeout(request, timeout);
                return new Response(ResponseCode.RequestTimeout);
            }

            var state = new HttpWebRequestState(timeout);

            using (var timeoutCancellation = new CancellationTokenSource())
            {
                var timeoutTask = Task.Delay(state.TimeRemaining, timeoutCancellation.Token);
                var senderTask = SendInternalAsync(request, state, cancellationToken);
                var completedTask = await Task.WhenAny(timeoutTask, senderTask).ConfigureAwait(false);
                if (completedTask is Task<Response> taskWithResponse)
                {
                    timeoutCancellation.Cancel();
                    return taskWithResponse.GetAwaiter().GetResult();
                }

                // (iloktionov): Если выполнившееся задание не кастуется к Task<Response>, сработал таймаут.
                state.CancelRequest();
                LogRequestTimeout(request, timeout);
                threadPoolMonitor.ReportAndFixIfNeeded(log);

                // (iloktionov): Попытаемся дождаться завершения задания по отправке запроса перед тем, как возвращать результат:
                await Task.WhenAny(senderTask.ContinueWith(_ => { }), Task.Delay(requestAbortTimeout)).ConfigureAwait(false);

                if (!senderTask.IsCompleted)
                    LogFailedToWaitForRequestAbort();

                return ResponseFactory.BuildResponse(ResponseCode.RequestTimeout, state);
            }
        }

        private async Task<Response> SendInternalAsync(Request request, HttpWebRequestState state, CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(state.CancelRequest))
            {
                for (state.ConnectionAttempt = 1; state.ConnectionAttempt <= ConnectionAttempts; state.ConnectionAttempt++)
                {
                    using (state)
                    {
                        if (state.RequestCancelled)
                            return new Response(ResponseCode.Canceled);

                        state.Reset();
                        state.Request = HttpWebRequestFactory.Create(request, state.TimeRemaining);

                        HttpActionStatus status;

                        // (iloktionov): Шаг 1 - отправить тело запроса, если оно имеется.
                        if (state.RequestCancelled)
                            return new Response(ResponseCode.Canceled);

                        if (request.Content != null)
                        {
                            status = await connectTimeLimiter.LimitConnectTime(SendRequestBodyAsync(request, state), request, state, ConnectionTimeout).ConfigureAwait(false);

                            if (status == HttpActionStatus.ConnectionFailure)
                                continue;

                            if (status != HttpActionStatus.Success)
                                return ResponseFactory.BuildFailureResponse(status, state);
                        }

                        // (iloktionov): Шаг 2 - получить ответ от сервера.
                        if (state.RequestCancelled)
                            return new Response(ResponseCode.Canceled);

                        status = request.Content != null
                            ? await GetResponseAsync(request, state).ConfigureAwait(false)
                            : await connectTimeLimiter.LimitConnectTime(GetResponseAsync(request, state), request, state, ConnectionTimeout).ConfigureAwait(false);

                        if (status == HttpActionStatus.ConnectionFailure)
                            continue;

                        if (status != HttpActionStatus.Success)
                            return ResponseFactory.BuildFailureResponse(status, state);

                        // (iloktionov): Шаг 3 - скачать тело ответа, если оно имеется.
                        if (!NeedToReadResponseBody(request, state))
                            return ResponseFactory.BuildSuccessResponse(state);

                        if (state.RequestCancelled)
                            return new Response(ResponseCode.Canceled);

                        status = await ReadResponseBodyAsync(request, state).ConfigureAwait(false);
                        return status == HttpActionStatus.Success
                            ? ResponseFactory.BuildSuccessResponse(state)
                            : ResponseFactory.BuildFailureResponse(status, state);
                    }
                }

                return new Response(ResponseCode.ConnectFailure);
            }
        }

        private async Task<HttpActionStatus> SendRequestBodyAsync(Request request, HttpWebRequestState state)
        {
            try
            {
                state.RequestStream = await state.Request.GetRequestStreamAsync().ConfigureAwait(false);
            }
            catch (WebException error)
            {
                return HandleWebException(request, state, error);
            }
            catch (Exception error)
            {
                LogUnknownException(error);
                return HttpActionStatus.UnknownFailure;
            }

            try
            {
                await state.RequestStream.WriteAsync(request.Content.Buffer, request.Content.Offset, request.Content.Length);
                state.CloseRequestStream();
            }
            catch (Exception error)
            {
                if (IsCancellationException(error))
                    return HttpActionStatus.RequestCanceled;

                LogSendBodyFailure(request, error);
                return HttpActionStatus.SendFailure;
            }

            return HttpActionStatus.Success;
        }

        private async Task<HttpActionStatus> GetResponseAsync(Request request, HttpWebRequestState state)
        {
            try
            {
                state.Response = (HttpWebResponse)await state.Request.GetResponseAsync().ConfigureAwait(false);
                state.ResponseStream = state.Response.GetResponseStream();
                return HttpActionStatus.Success;
            }
            catch (WebException error)
            {
                var status = HandleWebException(request, state, error);
                // (iloktionov): HttpWebRequest реагирует на коды ответа вроде 404 или 500 исключением со статусом ProtocolError.
                if (status == HttpActionStatus.ProtocolError)
                {
                    state.Response = (HttpWebResponse)error.Response;
                    state.ResponseStream = state.Response.GetResponseStream();
                    return HttpActionStatus.Success;
                }
                return status;
            }
            catch (Exception error)
            {
                LogUnknownException(error);
                return HttpActionStatus.UnknownFailure;
            }
        }

        private static bool NeedToReadResponseBody(Request request, HttpWebRequestState state)
        {
            if (request.Method == RequestMethods.Head)
                return false;

            // (iloktionov): ContentLength может быть равен -1, если сервер не укажет заголовок, но вернет контент. Это умолчание на уровне HttpWebRequest.
            return state.Response.ContentLength != 0;
        }

        private async Task<HttpActionStatus> ReadResponseBodyAsync(Request request, HttpWebRequestState state)
        {
            try
            {
                var contentLength = (int)state.Response.ContentLength;
                if (contentLength > 0)
                {
                    state.BodyBuffer = new byte[contentLength];

                    var totalBytesRead = 0;

                    // (iloktionov): Если буфер размером contentLength не попадет в LOH, можно передать его напрямую для работы с сокетом.
                    // В противном случае лучше использовать небольшой промежуточный буфер из пула, т.к. ссылка на переданный сохранится надолго из-за Keep-Alive.
                    if (contentLength < lohObjectSizeThreshold)
                    {
                        while (totalBytesRead < contentLength)
                        {
                            var bytesToRead = Math.Min(contentLength - totalBytesRead, preferredReadSize);
                            var bytesRead = await state.ResponseStream.ReadAsync(state.BodyBuffer, totalBytesRead, bytesToRead).ConfigureAwait(false);
                            if (bytesRead == 0)
                                break;

                            totalBytesRead += bytesRead;
                        }
                    }
                    else
                    {
                        using (var bufferHandle = readBuffersPool.AcquireHandle())
                        {
                            var buffer = bufferHandle.Resource;

                            while (totalBytesRead < contentLength)
                            {
                                var bytesToRead = Math.Min(contentLength - totalBytesRead, buffer.Length);
                                var bytesRead = await state.ResponseStream.ReadAsync(buffer, 0, bytesToRead).ConfigureAwait(false);
                                if (bytesRead == 0)
                                    break;

                                Buffer.BlockCopy(buffer, 0, state.BodyBuffer, totalBytesRead, bytesRead);

                                totalBytesRead += bytesRead;
                            }
                        }
                    }

                    if (totalBytesRead < contentLength)
                        throw new EndOfStreamException($"Response stream ended prematurely. Read only {totalBytesRead} byte(s), but Content-Length specified {contentLength}.");
                }
                else
                {
                    state.BodyStream = new MemoryStream();

                    using (var bufferHandle = readBuffersPool.AcquireHandle())
                    {
                        var buffer = bufferHandle.Resource;

                        while (true)
                        {
                            var bytesRead = await state.ResponseStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                            if (bytesRead == 0)
                                break;

                            state.BodyStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                return HttpActionStatus.Success;
            }
            catch (Exception error)
            {
                if (IsCancellationException(error))
                    return HttpActionStatus.RequestCanceled;

                LogReceiveBodyFailure(request, error);
                return HttpActionStatus.ReceiveFailure;
            }
        }

        private HttpActionStatus HandleWebException(Request request, HttpWebRequestState state, WebException error)
        {
            switch (error.Status)
            {
                case WebExceptionStatus.ConnectFailure:
                case WebExceptionStatus.KeepAliveFailure:
                case WebExceptionStatus.ConnectionClosed:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.NameResolutionFailure:
                case WebExceptionStatus.ProxyNameResolutionFailure:
                case WebExceptionStatus.SecureChannelFailure:
                    LogConnectionFailure(request, error, state.ConnectionAttempt);
                    return HttpActionStatus.ConnectionFailure;
                case WebExceptionStatus.SendFailure:
                    LogWebException(error);
                    return HttpActionStatus.SendFailure;
                case WebExceptionStatus.ReceiveFailure:
                    LogWebException(error);
                    return HttpActionStatus.ReceiveFailure;
                case WebExceptionStatus.RequestCanceled: return HttpActionStatus.RequestCanceled;
                case WebExceptionStatus.Timeout: return HttpActionStatus.Timeout;
                case WebExceptionStatus.ProtocolError: return HttpActionStatus.ProtocolError;
                default:
                    LogWebException(error);
                    return HttpActionStatus.UnknownFailure;
            }
        }

        private static bool IsCancellationException(Exception error)
        {
            return error is OperationCanceledException || (error as WebException)?.Status == WebExceptionStatus.RequestCanceled;
        }

        #region Logging

        private void LogRequestTimeout(Request request, TimeSpan timeout)
        {
            log.Error($"Request timed out. Target = {request.Url.Authority}. Timeout = {timeout.TotalSeconds:0.000} sec.");
        }

        private void LogConnectionFailure(Request request, WebException error, int attempt)
        {
            log.Error($"Connection failure. Target = {request.Url.Authority}. Attempt = {attempt}/{ConnectionAttempts}. Status = {error.Status}.", error.InnerException ?? error);
        }

        private void LogWebException(WebException error)
        {
            log.Error($"Error in sending request. Status = {error.Status}.", error.InnerException ?? error);
        }

        private void LogUnknownException(Exception error)
        {
            log.Error("Unknown error in sending request.", error);
        }

        private void LogSendBodyFailure(Request request, Exception error)
        {
            log.Error("Error in sending request body to " + request.Url.Authority, error);
        }

        private void LogReceiveBodyFailure(Request request, Exception error)
        {
            log.Error("Error in receiving request body from " + request.Url.Authority, error);
        }

        private void LogFailedToWaitForRequestAbort()
        {
            log.Warn($"Timed out request was aborted but did not complete in {requestAbortTimeout}.");
        }

        #endregion
    }
}