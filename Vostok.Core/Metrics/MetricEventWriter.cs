using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Metrics
{
    internal class MetricEventWriter : IMetricEventWriter
    {
        private readonly PoolHandle<MetricEvent> metricEventHandle;
        private readonly Action<MetricEvent> commit;

        private readonly Dictionary<string, string> tags;
        private readonly Dictionary<string, double> values;

        public MetricEventWriter(PoolHandle<MetricEvent> metricEventHandle, Action<MetricEvent> commit)
        {
            this.metricEventHandle = metricEventHandle;
            this.commit = commit;
            tags = (Dictionary<string, string>) metricEventHandle.Resource.Tags;
            values = (Dictionary<string, double>) metricEventHandle.Resource.Values;
            metricEventHandle.Resource.Timestamp = DateTimeOffset.UtcNow;
        }

        public IMetricEventWriter SetTimestamp(DateTimeOffset offset)
        {
            metricEvent.Timestamp = offset;
            return this;
        }

        public IMetricEventWriter SetTag(string key, string value)
        {
            tags[key] = value;
            return this;
        }

        public IMetricEventWriter SetValue(string key, double value)
        {
            values[key] = value;
            return this;
        }

        public void Commit()
        {
            commit(metricEvent);
        }
    }
}