using System;

namespace Vostok.Metrics
{
    public interface IMeterClock
    {
        TimeSpan Period { get; }
        void Register<T>(IMeter<T> meter, IMetricConverter<T> converter, IMetricRecorder recorder);
    }
}