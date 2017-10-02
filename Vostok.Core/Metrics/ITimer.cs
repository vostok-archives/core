using System;

namespace Vostok.Metrics
{
    public interface ITimer : IDisposable
    {
        void SetTag(string key, string value);
        void SetValue(string key, double value);
    }
}