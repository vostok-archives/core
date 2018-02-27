using System;
using System.Collections.Concurrent;

namespace Vstk.Metrics
{
    public static class MetricClocks
    {
        private static readonly ConcurrentDictionary<TimeSpan, MetricClock> clocks;

        static MetricClocks()
        {
            clocks = new ConcurrentDictionary<TimeSpan, MetricClock>();
        }

        public static MetricClock Get(TimeSpan period)
        {
            var clock = clocks.GetOrAdd(period, per => new MetricClock(per));
            clock.Start();
            return clock;
        }

        public static MetricClock Get()
        {
            var clock = clocks.GetOrAdd(MetricClock.DefaultPeriod, per => new MetricClock(per));
            clock.Start();
            return clock;
        }

        public static void Stop()
        {
            foreach (var kvp in clocks)
            {
                kvp.Value.Stop();
            }
        }
    }
}