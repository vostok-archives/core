using System;
using System.Threading.Tasks;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Weighed;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;

namespace Vostok.Airlock
{
    public class RequestSender : IRequestSender
    {
        private readonly AirlockConfig config;
        private readonly ClusterClient client;
        private readonly ILog log;

        public RequestSender(AirlockConfig config, ILog log)
        {
            this.config = config;
            this.log = log;

            client = new ClusterClient(log, configuration =>
            {
                configuration.ServiceName = "airlock";
                configuration.ClusterProvider = config.ClusterProvider;
                configuration.DefaultTimeout = config.RequestTimeout;
                configuration.DefaultRequestStrategy = Strategy.Forking2;
                configuration.EnableTracing = config.EnableTracing;

                configuration.SetupVostokHttpTransport();
                configuration.SetupWeighedReplicaOrdering(builder => builder.AddAdaptiveHealthModifierWithLinearDecay(10.Minutes()));
                configuration.SetupReplicaBudgeting(configuration.ServiceName);
                configuration.SetupAdaptiveThrottling(configuration.ServiceName);
            });
        }

        public async Task<RequestSendResult> SendAsync(ArraySegment<byte> serializedMessage)
        {
            var request = Request.Post("send")
                .WithHeader("x-apikey", config.ApiKey)
                .WithContent(serializedMessage);

            var result = await client.SendAsync(request, operationName: "send-records").ConfigureAwait(false);

            switch (result.Status)
            {
                case ClusterResultStatus.Success:
                    var response = result.Response;

                    if (!response.IsSuccessful && response.Content.Length > 0)
                    {
                        log.Warn($"Server error message: '{response.Content}'.");
                    }

                    return response.IsSuccessful ? RequestSendResult.Success : RequestSendResult.DefinitiveFailure;

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