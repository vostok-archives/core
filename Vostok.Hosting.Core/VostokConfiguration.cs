using System;
using Vostok.Airlock;
using Vostok.Airlock.Metrics;
using Vostok.Airlock.Tracing;
using Vostok.Clusterclient.Topology;
using Vostok.Logging;
using Vostok.Logging.Logs;
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

        public static class Airlock
        {
            private static readonly Lazy<IAirlockClient> lazyClient = new Lazy<IAirlockClient>(CreateAirlockClient);

            public static int Paralellizm { get; set; }

            public static AirlockConfig Config { get; } = new AirlockConfig {ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:8888/"))};

            public static IAirlockClient Client => lazyClient.Value;

            private static IAirlockClient CreateAirlockClient()
            {
                var log = Logging.GetLog("airlock");
                if (Paralellizm <= 1)
                    return new AirlockClient(Config, log);
                return new ParallelAirlockClient(Config, Paralellizm, log);
            }
        }

        public static class Logging
        {
            public static Func<string, ILog> GetLog { get; set; } = _ => new SilentLog();

            public static string RoutingKey => Vostok.Airlock.RoutingKey.TryCreate(Project(), Environment(), Service(), Vostok.Airlock.RoutingKey.LogsSuffix);
        }
    }
}