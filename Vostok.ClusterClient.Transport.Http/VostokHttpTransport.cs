using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    public class VostokHttpTransport : ITransport
    {
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
            throw new NotImplementedException();
        }
    }
}
