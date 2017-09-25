using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Airlock;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Topology;
using Vostok.Logging;

namespace Vostok.AirlockClient
{
    public class AirlockClient : IAirlockClient
    {
        private readonly ClusterClient _cluster;

        public AirlockClient(ILog log)
        {
            _cluster = new ClusterClient(log, config =>
            {
                // TODO: Accept endpoints in parameter
                config.ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:70000"));
                // TODO: Specify transport
            });
        }

        public async Task<bool> PingAsync(TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = Request.Head("ping");
            var result = await _cluster.SendAsync(request, timeout: timeout, cancellationToken: cancellationToken);
            return result.Status == ClusterResultStatus.Success;
        }

        public async Task<bool> SendAsync(AirlockMessage message, TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = Request.Post("send");
            var result = await _cluster.SendAsync(request, timeout: timeout, cancellationToken: cancellationToken);
            return result.Status == ClusterResultStatus.Success;
        }
    }
}