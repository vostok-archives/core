using Vostok.Airlock;

namespace Vostok.Metrics
{
    public class AirlockMetricReporter : IMetricEventReporter
    {
        private readonly IAirlockClient airlockClient;
        private readonly string metricRoutingKey;
        private readonly string metricEventRoutingKey;

        public AirlockMetricReporter(
            IAirlockClient airlockClient,
            IMetricConfiguration configuration)
        {
            this.airlockClient = airlockClient;
            metricEventRoutingKey = CreateRoutingKey(configuration, "metric_events");
            metricRoutingKey = CreateRoutingKey(configuration, "metrics");
        }

        public void SendEvent(MetricEvent metricEvent)
        {
            airlockClient.Push(metricEventRoutingKey, metricEvent, metricEvent.Timestamp);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            airlockClient.Push(metricRoutingKey, metricEvent, metricEvent.Timestamp);
        }

        private static string CreateRoutingKey(
            IMetricConfiguration configuration,
            string routingSuffix)
        {
            return string.IsNullOrEmpty(configuration.Environment) 
                ? routingSuffix 
                : $"{configuration.Environment}_{routingSuffix}";
        }
    }
}