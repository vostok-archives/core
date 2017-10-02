using System;
using System.Collections.Generic;
using Vostok.Airlock;

namespace Vostok.Metrics
{
    internal class AirlockMetricEventWriter : IMetricEventWriter
    {
        private readonly IAirlock airlock;
        private readonly string routingKey;
        private readonly MetricEvent metricEvent;

        public AirlockMetricEventWriter(
            IAirlock airlock,
            string routingKey)
        {
            this.airlock = airlock;
            this.routingKey = routingKey;
            this.metricEvent = new MetricEvent
            {
                Timestamp = DateTimeOffset.UtcNow,
                Tags = new Dictionary<string, string>(),
                Values = new Dictionary<string, double>()
            };
        }

        public void SetTimestamp(DateTimeOffset offset)
        {
            metricEvent.Timestamp = offset;
        }

        public void SetTag(string key, string value)
        {
            metricEvent.Tags[key] = value;
        }

        public void SetValue(string key, double value)
        {
            metricEvent.Values[key] = value;
        }

        public void Commit()
        {
            airlock.Push(routingKey, this);
        }
    }
}