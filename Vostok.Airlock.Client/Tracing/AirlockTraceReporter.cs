using Vostok.Tracing;

namespace Vostok.Airlock.Tracing
{
    public class AirlockTraceReporter : ITraceReporter
    {
        private readonly IAirlockClient airlockClient;
        private readonly string routingKey;

        public AirlockTraceReporter(
            IAirlockClient airlockClient,
            string routingKey)
        {
            this.airlockClient = airlockClient;
            this.routingKey = routingKey;
        }

        public void SendSpan(Span span)
        {
            airlockClient.Push(routingKey, span);
        }
    }
}