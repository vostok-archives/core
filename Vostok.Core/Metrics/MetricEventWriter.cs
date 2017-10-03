using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    internal class MetricEventWriter : IMetricEventWriter
    {
        private readonly Action<MetricEvent> commit;
        private readonly MetricEvent metricEvent;

        public MetricEventWriter(Action<MetricEvent> commit)
        {
            this.commit = commit;
            this.metricEvent = new MetricEvent
            {
                Timestamp = DateTimeOffset.UtcNow,
                Tags = new Dictionary<string, string>(),
                Values = new Dictionary<string, double>()
            };
        }

        public IMetricEventWriter SetTimestamp(DateTimeOffset offset)
        {
            metricEvent.Timestamp = offset;
            return this;
        }

        public IMetricEventWriter SetTag(string key, string value)
        {
            metricEvent.Tags[key] = value;
            return this;
        }

        public IMetricEventWriter SetValue(string key, double value)
        {
            metricEvent.Values[key] = value;
            return this;
        }

        public void Commit()
        {
            commit(metricEvent);
        }
    }
}