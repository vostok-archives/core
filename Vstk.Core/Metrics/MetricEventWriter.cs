using System;
using System.Collections.Generic;
using Vstk.Commons;
using Vstk.Commons.Collections;
using Vstk.Flow;

namespace Vstk.Metrics
{
    internal class MetricEventWriter : IMetricEventWriter
    {
        private readonly IPool<MetricEventWriter> originPool;
        private readonly IMetricConfiguration configuration;
        private readonly Action<MetricEvent> commit;

        private readonly Dictionary<string, string> tags;
        private readonly Dictionary<string, double> values;
        private readonly MetricEvent metricEvent;

        public MetricEventWriter(
            IPool<MetricEventWriter> originPool,
            IMetricConfiguration configuration,
            Action<MetricEvent> commit)
        {
            this.originPool = originPool;
            this.configuration = configuration;
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

        public void Initialize()
        {
            SetTimestamp(DateTimeOffset.UtcNow);
            EnrichWithContext();
            EnrichWithHostname();
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

        private void EnrichWithContext()
        {
            foreach (var pair in Context.Properties.Current)
            {
                if (configuration.ContextFieldsWhitelist.Contains(pair.Key))
                    SetTag(pair.Key, Convert.ToString(pair.Value));
            }
        }

        private void EnrichWithHostname()
        {
            SetTag(MetricsTagNames.Host, HostnameProvider.Get());
        }
    }
}