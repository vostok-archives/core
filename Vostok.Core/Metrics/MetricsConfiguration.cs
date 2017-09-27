using System;

namespace Vostok.Metrics
{
    public class MetricsConfiguration
    {
        public IMetricRecorder DefaultMetricsRecorder { get; set; }
        public Func<string, string>  DefaultMetricNameTransformer { get; set; }
    }
}