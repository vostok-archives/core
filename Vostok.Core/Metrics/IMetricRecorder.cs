using System.Collections.Generic;

namespace Vostok.Metrics
{
    public interface IMetricRecorder
    {
        void Record(IEnumerable<Metric> values);
    }
}