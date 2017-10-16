using System;
using Vostok.Tracing;

namespace Vostok.Airlock.Tracing
{
    public class AirlockTraceReporter : ITraceReporter
    {
        private readonly Func<IAirlockClient> getAirlockClient;
        private readonly Func<string> getRoutingKey;

        public AirlockTraceReporter(IAirlockClient airlockClient, string routingKey)
            : this(() => airlockClient, () => routingKey)
        {
        }

        public AirlockTraceReporter(Func<IAirlockClient> getAirlockClient, Func<string> getRoutingKey)
        {
            this.getAirlockClient = getAirlockClient;
            this.getRoutingKey = getRoutingKey;
        }

        public void SendSpan(Span span)
        {
            var airlockClient = getAirlockClient();
            var routingKey = getRoutingKey();
            if (airlockClient == null || string.IsNullOrEmpty(routingKey))
                return;
            airlockClient.Push(routingKey, span);
        }
    }
}