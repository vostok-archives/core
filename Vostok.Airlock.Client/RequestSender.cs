using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Weighed;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Commons.Extensions.UnitConvertions;

namespace Vostok.Airlock
{
    internal class RequestSender : IRequestSender
    {
        private readonly AirlockConfig config;
        private readonly ClusterClient client;

        public RequestSender(AirlockConfig config)
        {
            this.config = config;

            client = new ClusterClient(config.Log, configuration =>
            {
                configuration.ServiceName = "airlock";
                configuration.ClusterProvider = config.ClusterProvider;
                configuration.DefaultTimeout = config.RequestTimeout;
                configuration.DefaultRequestStrategy = Strategy.Forking2;

                configuration.SetupVostokHttpTransport();
                configuration.SetupWeighedReplicaOrdering(builder => builder.AddAdaptiveHealthModifierWithLinearDecay(10.Minutes()));
                configuration.SetupReplicaBudgeting(configuration.ServiceName);
                configuration.SetupAdaptiveThrottling(configuration.ServiceName);
            });
        }

        public async Task<RequestSendResult> SendAsync(ArraySegment<byte> serializedMessage)
        {
            var request = Request.Post("send")
                .WithHeader("apikey", config.ApiKey)
                .WithContent(serializedMessage);

            var result = await client.SendAsync(request).ConfigureAwait(false);

            switch (result.Status)
            {
                case ClusterResultStatus.Success:
                    return result.Response.IsSuccessful ? RequestSendResult.Success : RequestSendResult.DefinitiveFailure;

                case ClusterResultStatus.TimeExpired:
                case ClusterResultStatus.ReplicasExhausted:
                case ClusterResultStatus.Throttled:
                    return RequestSendResult.IntermittentFailure;

                default:
                    return RequestSendResult.DefinitiveFailure;
            }
        }
    }
}