using System;

namespace Vostok.Metrics
{
    public interface IMetricEventWriter
    {
        void SetTimestamp(DateTimeOffset offset);
        void SetTag(string key, string value);
        void SetValue(string key, double value);
        void Commit();
    }
}