using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities.Convertions.Time;
using Vostok.Logging;

namespace Vostok.ClusterClient.Transport.Http
{
    /// <summary>
    /// <para>Represents an <see cref="ITransport"/> implementation which uses <see cref="System.Net.Http.HttpClient"/> to send requests to replicas.</para>
    /// <para>You can pass an instance of <see cref="HttpClientSettings"/> to constructor to tune client behaviour.</para>
    /// <para>You can also use <see cref="SetupKonturHttpTransport(Vostok.Clusterclient.IClusterClientConfiguration,Vostok.Http.Client.HttpClientSettings)"/> extension to set up this transport in your configuration.</para>
    /// </summary>
    public class VostokHttpTransport : ITransport
    {
        private readonly IHttpClient httpClient;

        public VostokHttpTransport(ILog log)
            : this(CreateDefaultClientSettings(), log)
        {
        }

        public VostokHttpTransport(HttpClientSettings clientSettings, ILog log)
            : this(new HttpClient(clientSettings, log))
        {
        }

        internal VostokHttpTransport(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return await httpClient.SendAsync(request, timeout, cancellationToken).ConfigureAwait(false);
        }

        private static HttpClientSettings CreateDefaultClientSettings()
        {
            return HttpClientSettingsBuilder
                .StartNew()
                .EnableConnectTimeout(750.Milliseconds())
                .SetConnectionAttempts(2)
                .Build();
        }
    }
}
