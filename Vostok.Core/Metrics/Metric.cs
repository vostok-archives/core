namespace Vostok.Metrics
{
    public static class Metric
    {
        private static readonly IMetricConfiguration configuration;

        static Metric()
        {
            configuration = new MetricConfiguration();
            Root = new RootMetricScope(configuration);
        }

        public static IMetricScope Root { get; }

        public static IMetricConfiguration Configuration => configuration;
    }
}