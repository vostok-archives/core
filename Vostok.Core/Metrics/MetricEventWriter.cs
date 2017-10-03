using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    internal class MetricEventWriter : IMetricEventWriter
    {
        private readonly Action<MetricEvent> commit;
        private readonly MetricEvent metricEvent;

        private readonly Dictionary<string, string> tags;
        private readonly Dictionary<string, double> values;

        public MetricEventWriter(Action<MetricEvent> commit)
        {
            this.commit = commit;
            tags = new Dictionary<string, string>();
            values = new Dictionary<string, double>();
            //TODO (@ezsilmar) Do it more efficiently
            this.metricEvent = new MetricEvent
            {
                Timestamp = DateTimeOffset.UtcNow,
                Tags = tags,
                Values = values
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
        }
    }
}