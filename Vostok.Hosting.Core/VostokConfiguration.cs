using System;
using Vostok.Airlock;
using Vostok.Airlock.Metrics;
using Vostok.Airlock.Tracing;
using Vostok.Metrics;
using Vostok.Tracing;

namespace Vostok.Hosting
{
    public static class VostokConfiguration
    {
        static VostokConfiguration()
        {
            Metrics.Reporter = new AirlockMetricReporter(() => Airlock.Client, () => RoutingKey.TryCreatePrefix(Project(), Environment(), Service()));
            Tracing.Reporter = new AirlockTraceReporter(() => Airlock.Client, () => RoutingKey.TryCreate(Project(), Environment(), Service(), RoutingKey.TracesSuffix));
        }

        public static Func<string> Project { get; set; }

        public static Func<string> Environment { get; set; }

        public static Func<string> Service { get; set; }

        public static ITraceConfiguration Tracing => Trace.Configuration;

        public static IMetricConfiguration Metrics => Metric.Configuration;

        public static VostokAirlockConfiguration Airlock { get; } = new VostokAirlockConfiguration();

        public static VostokLoggingConfiguration Logging { get; } = new VostokLoggingConfiguration();
    }
}