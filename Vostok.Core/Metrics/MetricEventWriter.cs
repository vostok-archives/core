using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Metrics
{
    internal class MetricEventWriter : IMetricEventWriter
    {
        private readonly IPool<MetricEventWriter> originPool;
        private readonly Action<MetricEvent> commit;

        private readonly Dictionary<string, string> tags;
        private readonly Dictionary<string, double> values;
        private readonly MetricEvent metricEvent;

        public MetricEventWriter(IPool<MetricEventWriter> originPool, Action<MetricEvent> commit)
        {
            this.originPool = originPool;
            this.commit = commit;
            tags = new Dictionary<string, string>();
            values = new Dictionary<string, double>();
            metricEvent = new MetricEvent
            {
                Tags = tags,
                Values = values,
                Timestamp = DateTimeOffset.UtcNow
            };
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
            Cleanup();
            originPool.Release(this);
        }

        private void Cleanup()
        {
            tags.Clear();
            values.Clear();
        }
    }
}