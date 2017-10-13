namespace Vostok.Metrics
{
    public static class Metric
    {
        static Metric()
        {
            var metricConfiguration = new DefaultMetricConfiguration();
            Root = new RootMetricScope(metricConfiguration);
        }

        public static IMetricScope Root { get; }
    }
}