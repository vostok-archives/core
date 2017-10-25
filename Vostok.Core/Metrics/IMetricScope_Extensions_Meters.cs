using System;
using Vostok.Metrics.Meters;

namespace Vostok.Metrics
{
    public static class IMetricScope_Extensions_Meters
    {
        public static IEventStopwatch EventStopwatch(
            this IMetricScope scope)
        {
            return new EventStopwatch(scope);
        }

        public static IDisposable Gauge(
            this IMetricScope scope,
            TimeSpan period,
            string name,
            Func<double> getValue)
        {
            var clock = MetricClocks.Get(period);
            Action<DateTimeOffset> action = timestamp =>
            {
                var value = getValue();
                SendMetricToScope(scope, name, timestamp, value);
            };
            clock.Register(action);
            return new DisposableMetric(clock, action);
        }

        private class DisposableMetric : IDisposable
        {
            private readonly MetricClock clock;
            private readonly Action<DateTimeOffset> action;

            public DisposableMetric(MetricClock clock, Action<DateTimeOffset> action)
            {
                this.clock = clock;
                this.action = action;
            }

            public void Dispose()
            {
                clock.Unregister(action);
            }
        }

        private class DisposableCounter : DisposableMetric, ICounter
        {
            private readonly ICounter counter;

            public DisposableCounter(MetricClock clock, Action<DateTimeOffset> action, ICounter counter)
                : base(clock, action)
            {
                this.counter = counter;
            }

            public void Add(long value = 1)
            {
                counter.Add(value);
            }
        }

        public static ICounter Counter(
            this IMetricScope scope,
            TimeSpan period,
            string name)
        {
            var counter = new Counter();
            var clock = MetricClocks.Get(period);
            Action<DateTimeOffset> action = timestamp =>
            {
                var value = counter.Reset();
                SendMetricToScope(scope, name, timestamp, value);
            };
            clock.Register(
                action);
            return new DisposableCounter(clock, action, counter);
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