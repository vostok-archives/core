using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    // TODO(iloktionov): 1. Tune CurlHandler in case it backs our handler (see SetCurlOption function with CURLOPT_CONNECTTIMEOUT_MS)
    // TODO(iloktionov): 2. Classify errors from CurlHandler (they are CurlExceptions, see Interop.CURLcode in corefx)
    public partial class VostokHttpTransport : IDisposable
    {
        private readonly ILog log;
        private readonly HttpClientHandler handler;
        private readonly HttpClient httpClient;
        private TimeSpan? connectionTimeout;

        public VostokHttpTransport(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));

            connectionTimeout = 2.Seconds();

            handler = CreateClientHandler();

            TuneClientHandler();

            httpClient = new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public TimeSpan? ConnectionTimeout
        {
            get => connectionTimeout;
            set
            {
                connectionTimeout = value;
                TuneClientHandler();
            }
        }

        public async Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout.TotalMilliseconds < 1)
            {
                LogRequestTimeout(request, timeout);
                return new Response(ResponseCode.RequestTimeout);
            }

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
                ServerCertificateCustomValidationCallback = null
            };

            // (alexkir, 13.10.2017) we can safely pass callbacks only on Windows; see https://github.com/dotnet/corefx/pull/19908
            if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
                Environment.OSVersion.Platform == PlatformID.Win32S ||
                Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                Environment.OSVersion.Platform == PlatformID.WinCE)
                // TODO(alexkir, 13.10.2017): decide if this is a good optimization practice to always ignore SSL cert validity
                handler.ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true;

            return handler;
        }

        private void TuneClientHandler()
        {
            WinHttpHandlerTuner.Tune(handler, ConnectionTimeout, log);
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