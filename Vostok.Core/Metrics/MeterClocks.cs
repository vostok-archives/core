using System;

namespace Vostok.Metrics
{
    public static class MeterClocks
    {
        private static readonly MetricsConfiguration configuration;

        static MeterClocks()
        {
            configuration = new MetricsConfiguration();
        }

        public static IMeterClock Get(TimeSpan clockPeriod)
        {
            // use one MeterClock for clockPeriod
            return null;
        }

        public static Counter CreateCounter(this IMeterClock clock, string name, IMetricRecorder recorder = null)
        {
            var counter = new Counter();
            clock.Register(
                counter,
                GetNameConverter(name),
                recorder ?? configuration.DefaultMetricsRecorder);
            return counter;
        }

        public static void RegisterGauge(this IMeterClock clock, string name, Func<double> valueFunc)
        {
            var gauge = new GaugeMeter(valueFunc);
            clock.Register(
                gauge,
                GetNameConverter(name),
                configuration.DefaultMetricsRecorder);
        }

        private static NameValueConverter GetNameConverter(string name)
        {
            return new NameValueConverter(configuration.DefaultMetricNameTransformer(name));
        }
    }
}