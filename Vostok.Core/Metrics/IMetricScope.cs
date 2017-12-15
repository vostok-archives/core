using System;

namespace Vostok.Metrics
{
    public interface IMetricScope
    {
        IMetricEventWriter WriteEvent();
        IMetricEventWriter WriteMetric();
        TimeSpan DefaultInterval { get; }
    }
}