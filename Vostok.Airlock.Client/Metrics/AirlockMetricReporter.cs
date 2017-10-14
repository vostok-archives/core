using Vostok.Metrics;

namespace Vostok.Airlock.Metrics
{
    public class AirlockMetricReporter : IMetricEventReporter
    {
        private readonly IAirlockClient airlockClient;
        private readonly string metricsRoutingKey;
        private readonly string appEventsRoutingKey;

        public AirlockMetricReporter(
            IAirlockClient airlockClient,
            string routingKeyPrefix)
        {
            this.airlockClient = airlockClient;
            appEventsRoutingKey = RoutingKey.AddSuffix(routingKeyPrefix, RoutingKey.AppEventsSuffix);
            metricsRoutingKey = RoutingKey.AddSuffix(routingKeyPrefix, RoutingKey.MetricsSuffix);
        }

        public void SendEvent(MetricEvent metricEvent)
        {
            airlockClient.Push(appEventsRoutingKey, metricEvent, metricEvent.Timestamp);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            airlockClient.Push(metricsRoutingKey, metricEvent, metricEvent.Timestamp);
        }
    }
}