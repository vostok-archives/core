using System;

namespace Vostok.Metrics
{
    public interface IEventStopwatch : IDisposable
    {
        void SetTag(string key, string value);
        void SetValue(string key, double value);
    }
}