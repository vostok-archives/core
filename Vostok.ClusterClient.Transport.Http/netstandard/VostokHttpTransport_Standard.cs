using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    // TODO(iloktionov): 1. Tune CurlHandler in case it backs our handler (see SetCurlOption function with CURLOPT_CONNECTTIMEOUT_MS)
    // TODO(iloktionov): 2. Classify errors from CurlHandler (they are CurlExceptions, see Interop.CURLcode in corefx)
    // TODO(iloktionov): 3. Functional tests.
    public partial class VostokHttpTransport : IDisposable
    {
        private readonly ILog log;
        private readonly HttpClientHandler handler;
        private readonly HttpClient httpClient;

        public VostokHttpTransport(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));

            handler = CreateClientHandler();

            TuneClientHandler();

            httpClient = new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public TimeSpan? ConnectionTimeout { get; set; } = TimeSpan.FromMilliseconds(500);

        public async Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                var requestMessage = SystemNetHttpRequestConverter.Convert(request);

                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    cts.CancelAfter(timeout);

                    var responseMessage = await httpClient.SendAsync(requestMessage, cts.Token).ConfigureAwait(false);

                    var response = await SystemNetHttpResponseConverter.ConvertAsync(responseMessage).ConfigureAwait(false);

                    return response;
                }
            }
            catch (OperationCanceledException)
            {
                return HandleCancellationError(request, timeout, cancellationToken);
            }
            catch (Exception error)
            {
                return HandleGenericError(request, error);
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private static HttpClientHandler CreateClientHandler()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
                CheckCertificateRevocationList = false,
                MaxConnectionsPerServer = 10000,
                Proxy = null,
                PreAuthenticate = false,
                UseDefaultCredentials = false,
                UseCookies = false,
                UseProxy = false,
                ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
            };

            return handler;
        }

        private void TuneClientHandler()
        {
            if (ConnectionTimeout.HasValue)
                WinHttpHandlerTuner.Tune(handler, ConnectionTimeout.Value, log);
        }

        private Response HandleCancellationError(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return new Response(ResponseCode.Canceled);

            LogRequestTimeout(request, timeout);

            return new Response(ResponseCode.RequestTimeout);
        }

        private Response HandleGenericError(Request request, Exception error)
        {
            error = error.InnerException ?? error;

            if (error is Win32Exception win32Error)
                return HandleWin32Error(request, win32Error);

            LogUnknownException(request, error);

            return new Response(ResponseCode.UnknownFailure);
        }

        private Response HandleWin32Error(Request request, Win32Exception error)
        {
            LogWin32Error(request, error);

            return WinHttpErrorsHandler.Handle(error);
        }

        #region Logging

        private void LogRequestTimeout(Request request, TimeSpan timeout)
        {
            log.Error($"Request timed out. Target = {request.Url.Authority}. Timeout = {timeout.TotalSeconds:0.000} sec.");
        }

        private void LogUnknownException(Request request, Exception error)
        {
            log.Error($"Unknown error in sending request to {request.Url.Authority}. ", error);
        }

        private void LogWin32Error(Request request, Win32Exception error)
        {
            log.Error($"WinAPI error with code {error.NativeErrorCode} while sending request to {request.Url.Authority}.", error);
        }

        #endregion
    }
}