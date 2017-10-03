using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    public static class IMetricScope_Extensions
    {
        public static IMetricScope WithTag(
            this IMetricScope scope,
            string key,
            string value)
        {
            return new MetricScopeTagEnricher(
                scope,
                new Dictionary<string, string> {{key, value}});
        }

        public static IMetricScope WithTags(
            this IMetricScope scope,
            IReadOnlyDictionary<string, string> tags)
        {
            return new MetricScopeTagEnricher(scope, tags);
        }

        public static IEventStopwatch EventStopwatch(
            this IMetricScope scope)
        {
            return new EventStopwatch(scope);
        }

        public static void Gauge(
            this IMetricScope scope,
            TimeSpan period,
            string name,
            Func<double> getValue)
        {
            var clock = MetricClocks.Get(period);
            clock.Register(
                timestamp =>
                {
                    var value = getValue();
                    SendMetricToScope(scope, name, timestamp, value);
                });
        }

        public static ICounter Counter(
            this IMetricScope scope,
            TimeSpan period,
            string name)
        {
            var counter = new Counter();
            var clock = MetricClocks.Get(period);
            clock.Register(
                timestamp =>
                {
                    var value = counter.Reset();
                    SendMetricToScope(scope, name, timestamp, value);
                });
            return counter;
        }

        private static void SendMetricToScope(
            IMetricScope scope,
            string name,
            DateTimeOffset timestamp,
            double value)
        {
            scope
                .WriteMetric()
                .SetTimestamp(timestamp)
                .SetValue(name, value)
                .Commit();
        }
    }
}