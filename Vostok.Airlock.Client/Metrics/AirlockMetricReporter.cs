using Vostok.Metrics;

namespace Vostok.Airlock.Metrics
{
    public class AirlockMetricReporter : IMetricEventReporter
    {
        private readonly IAirlockClient airlockClient;
        private readonly string routingKeyPrefix;

        public AirlockMetricReporter(IAirlockClient airlockClient, string routingKeyPrefix)
        {
            this.airlockClient = airlockClient;
            this.routingKeyPrefix = routingKeyPrefix;
        }

        public void SendEvent(MetricEvent metricEvent)
        {
            var routingKey = RoutingKey.TryAddSuffix(routingKeyPrefix, RoutingKey.AppEventsSuffix);
            if (airlockClient == null || string.IsNullOrEmpty(routingKey))
                return;
            airlockClient.Push(routingKey, metricEvent, metricEvent.Timestamp);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            var routingKey = RoutingKey.TryAddSuffix(routingKeyPrefix, RoutingKey.MetricsSuffix);
            if (airlockClient == null || string.IsNullOrEmpty(routingKey))
                return;
            airlockClient.Push(routingKey, metricEvent, metricEvent.Timestamp);
        }
    }
}