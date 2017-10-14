using System;
using Vostok.Airlock;

namespace Vostok.Metrics
{
    public class AirlockMetricReporter : IMetricEventReporter
    {
        private readonly Func<IAirlockClient> airlockClient;
        private readonly Func<string> metricsRoutingKey;
        private readonly Func<string> appEventsRoutingKey;

        public AirlockMetricReporter(IAirlockClient airlockClient, string routingKeyPrefix)
            : this(() => airlockClient, () => routingKeyPrefix)
        {
        }

        public AirlockMetricReporter(
            Func<IAirlockClient> airlockClient,
            Func<string> routingKeyPrefix)
        {
            this.airlockClient = airlockClient;
            appEventsRoutingKey = () => RoutingKey.AddSuffix(routingKeyPrefix(), RoutingKey.AppEventsSuffix);
            metricsRoutingKey = () => RoutingKey.AddSuffix(routingKeyPrefix(), RoutingKey.MetricsSuffix);
        }

        public void SendEvent(MetricEvent metricEvent)
        {
            airlockClient().Push(appEventsRoutingKey(), metricEvent, metricEvent.Timestamp);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            airlockClient().Push(metricsRoutingKey(), metricEvent, metricEvent.Timestamp);
        }
    }
}