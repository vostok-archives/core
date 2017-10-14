using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport
{
    internal class TransportWithAuxiliaryHeaders : ITransport
    {
        private static readonly string clientIdentity;

        private readonly ITransport transport;
        private readonly IClusterClientConfiguration configuration;

        static TransportWithAuxiliaryHeaders()
        {
            try
            {
                clientIdentity = Process.GetCurrentProcess().ProcessName;
            }
            catch
            {
                clientIdentity = "unknown";
            }
        }

        public TransportWithAuxiliaryHeaders(ITransport transport, IClusterClientConfiguration configuration)
        {
            this.transport = transport;
            this.configuration = configuration;
        }

        public Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (configuration.IncludeRequestTimeoutHeader)
            {
                request = request.WithHeader(HeaderNames.XKonturRequestTimeout, timeout.Ticks);
            }

            if (configuration.IncludeClientIdentityHeader)
            {
                request = request.WithHeader(HeaderNames.XKonturClientIdentity, clientIdentity);
            }

            return transport.SendAsync(request, timeout, cancellationToken);
        }
    }
}