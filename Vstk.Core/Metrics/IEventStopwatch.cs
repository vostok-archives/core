using System;

namespace Vstk.Metrics
{
    public interface IEventStopwatch : IDisposable
    {
        void SetTag(string key, string value);
        void SetValue(string key, double value);
    }
}