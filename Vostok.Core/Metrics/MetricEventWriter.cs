using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    internal class MetricEventWriter : IMetricEventWriter
    {
        private readonly Action<MetricEvent> commit;
        private DateTimeOffset timestamp;
        private readonly Dictionary<string, string> tags;
        private readonly Dictionary<string, double> values;

        public MetricEventWriter(Action<MetricEvent> commit)
        {
            this.commit = commit;
            timestamp = DateTimeOffset.UtcNow;
            tags = new Dictionary<string, string>();
            values = new Dictionary<string, double>();
        }

        public IMetricEventWriter SetTimestamp(DateTimeOffset timestamp)
        {
            this.timestamp = timestamp;
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
            commit(new MetricEvent
            {
                Timestamp = timestamp,
                Tags = tags,
                Values = values
            });
        }
    }
}