using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    public interface IMetricConverter<TMetric>
    {
        IEnumerable<Metric> Convert(TMetric metric, DateTimeOffset timestamp);
    }
}