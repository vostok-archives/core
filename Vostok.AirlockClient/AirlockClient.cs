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
        private readonly ILog _log;
        private readonly ClusterClient _cluster;

        public AirlockClient(ILog log)
        {
            _log = log;
            _cluster = new ClusterClient(log, config =>
            {
                // TODO: Accept endpoints in parameter
                config.ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:70000"));
                // TODO: Specify transport
            });
        }

        public async Task<AirlockResponse> PingAsync(TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var request = Request.Head("ping");
                // TODO: Choose request strategy
                var response = await _cluster.SendAsync(request, timeout: timeout, cancellationToken: cancellationToken);
                return AirlockResponse.FromClusterResult(response);
            }
            catch (Exception e)
            {
                _log.Error(e, "Ping failed with exception");
                return AirlockResponse.Exception(e);
            }
        }

        public async Task<AirlockResponse> SendAsync(AirlockMessage message, TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var request = Request.Post("send");
                // TODO: Set content
                // TODO: Choose request strategy
                var response = await _cluster.SendAsync(request, timeout: timeout, cancellationToken: cancellationToken);
                return AirlockResponse.FromClusterResult(response);
            }
            catch (Exception e)
            {
                _log.Error(e, "Send failed with exception");
                return AirlockResponse.Exception(e);
            }
        }
    }
}