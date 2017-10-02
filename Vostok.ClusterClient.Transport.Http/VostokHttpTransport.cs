using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Collections;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    // ReSharper disable MethodSupportsCancellation

    public class VostokHttpTransport : ITransport
    {
        private const int PreferredReadSize = 16*1024;
        private const int LOHObjectSizeThreshold = 85*1000;

        private static readonly TimeSpan RequestAbortTimeout = TimeSpan.FromMilliseconds(250);
        private static readonly IPool<byte[]> ReadBuffersPool = new UnlimitedLazyPool<byte[]>(() => new byte[PreferredReadSize]);

        private readonly VostokHttpTransportSettings settings;
        private readonly ILog log;

        public VostokHttpTransport(ILog log)
            : this(new VostokHttpTransportSettings(), log)
        {
        }

        public VostokHttpTransport(VostokHttpTransportSettings settings, ILog log)
        {
            this.settings = settings;
            this.log = log;
        }

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
                // TODO(iloktionov): fix threadpool if needed

                // (iloktionov): Попытаемся дождаться завершения задания по отправке запроса перед тем, как возвращать результат:
                await Task.WhenAny(senderTask.ContinueWith(_ => {}), Task.Delay(RequestAbortTimeout)).ConfigureAwait(false);

                if (!senderTask.IsCompleted)
                    LogFailedToWaitForRequestAbort();

                return ResponseFactory.BuildResponse(ResponseCode.RequestTimeout, state);
            }
        }

        private async Task<Response> SendInternalAsync(Request request, HttpWebRequestState state, CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(state.CancelRequest))
            {
                for (state.ConnectionAttempt = 1; state.ConnectionAttempt <= settings.ConnectionAttempts; state.ConnectionAttempt++)
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

                        // TODO(iloktionov): ...
                    }
                }
            }

            throw new NotImplementedException();
        }

        #region Logging

        private void LogRequestTimeout(Request request, TimeSpan timeout)
        {
            log.Error($"Request timed out. Target = {request.Url.Authority}. Timeout = {timeout.TotalSeconds:0.000} sec.");
        }

        private void LogConnectionFailure(Request request, WebException error, int attempt)
        {
            log.Error($"Connection failure. Target = {request.Url.Authority}. Attempt = {attempt}/{settings.ConnectionAttempts}. Status = {error.Status}.", error.InnerException ?? error);
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
            log.Warn($"Timed out request was aborted but did not complete in {RequestAbortTimeout}.");
        }

        #endregion
    }
}
