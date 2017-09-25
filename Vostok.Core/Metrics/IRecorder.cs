using System;

namespace Vostok.Metrics
{
    public interface IRecorder<TValue>
    {
        void Record(TValue value, DateTimeOffset timestamp);
    }
}