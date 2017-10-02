using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    // TODO(iloktionov): log timeouts
    // TODO(iloktionov): log cancellations

    public partial class VostokHttpTransport : IDisposable
    {
        private readonly ILog log;
        private readonly HttpClient httpClient;

        public VostokHttpTransport(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));

            httpClient = new HttpClient(CreateClientHandler())
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

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
                return cancellationToken.IsCancellationRequested
                    ? new Response(ResponseCode.Canceled) 
                    : new Response(ResponseCode.RequestTimeout);
            }
            catch (Exception error)
            {
                log.Error(error);

                return new Response(ResponseCode.UnknownFailure);
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private static HttpClientHandler CreateClientHandler()
        {
            return new HttpClientHandler
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
        }
    }
}