using System;
using Vostok.Metrics;

namespace Vostok.Airlock.Metrics
{
    public class AirlockMetricReporter : IMetricEventReporter
    {
        private readonly Func<IAirlockClient> getAirlockClient;
        private readonly Func<string> getMetricsRoutingKey;
        private readonly Func<string> getAppEventsRoutingKey;

        public AirlockMetricReporter(IAirlockClient airlockClient, string routingKeyPrefix)
            : this(() => airlockClient, () => routingKeyPrefix)
        {
        }

        public AirlockMetricReporter(Func<IAirlockClient> getAirlockClient, Func<string> routingKeyPrefix)
        {
            this.getAirlockClient = getAirlockClient;
            getAppEventsRoutingKey = () => RoutingKey.TryAddSuffix(routingKeyPrefix(), RoutingKey.AppEventsSuffix);
            getMetricsRoutingKey = () => RoutingKey.TryAddSuffix(routingKeyPrefix(), RoutingKey.MetricsSuffix);
        }

        public void SendEvent(MetricEvent metricEvent)
        {
            var airlockClient = getAirlockClient();
            var routingKey = getAppEventsRoutingKey();
            if (airlockClient == null || string.IsNullOrEmpty(routingKey))
                return;
            airlockClient.Push(routingKey, metricEvent, metricEvent.Timestamp);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            var airlockClient = getAirlockClient();
            var routingKey = getMetricsRoutingKey();
            if (airlockClient == null || string.IsNullOrEmpty(routingKey))
                return;
            airlockClient.Push(routingKey, metricEvent, metricEvent.Timestamp);
        }
    }
}