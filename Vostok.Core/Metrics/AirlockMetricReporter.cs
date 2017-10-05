using Vostok.Airlock;

namespace Vostok.Metrics
{
    public class AirlockMetricReporter : IMetricEventReporter
    {
        private readonly IAirlock airlock;
        private readonly string metricRoutingKey;
        private readonly string metricEventRoutingKey;

        public AirlockMetricReporter(
            IAirlock airlock,
            IMetricConfiguration configuration)
        {
            this.airlock = airlock;
            metricEventRoutingKey = CreateRoutingKey(configuration, "metric_events");
            metricRoutingKey = CreateRoutingKey(configuration, "metrics");
        }

        public void SendEvent(MetricEvent metricEvent)
        {
            airlock.Push(metricEventRoutingKey, metricEvent, metricEvent.Timestamp);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            airlock.Push(metricRoutingKey, metricEvent, metricEvent.Timestamp);
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