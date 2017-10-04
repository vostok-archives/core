using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Vostok.Commons;
using Vostok.Commons.Collections;
using Vostok.Flow;

namespace Vostok.Metrics
{
    public class RootMetricScope : IMetricScope
    {
        private readonly IMetricConfiguration configuration;
        private readonly UnlimitedLazyPool<MetricEventWriter> eventWriterPool;
        private readonly UnlimitedLazyPool<MetricEventWriter> metricWriterPool;

        public RootMetricScope(IMetricConfiguration configuration)
        {
            this.configuration = configuration;
            this.eventWriterPool = new UnlimitedLazyPool<MetricEventWriter>(
                () => new MetricEventWriter(
                    eventWriterPool,
                    metricEvent => configuration.Reporter?.SendEvent(metricEvent)));
            this.metricWriterPool = new UnlimitedLazyPool<MetricEventWriter>(
                () => new MetricEventWriter(
                    metricWriterPool,
                    metricEvent => configuration.Reporter?.SendMetric(metricEvent)));
        }

        public IMetricEventWriter WriteEvent()
        {
            var writer = eventWriterPool.Acquire();
            EnrichWithCommonFields(writer);
            return writer;
        }

        public IMetricEventWriter WriteMetric()
        {
            var writer = metricWriterPool.Acquire();
            EnrichWithCommonFields(writer);
            return writer;
        }

        private void EnrichWithCommonFields(MetricEventWriter writer)
        {
            SetTimestamp(writer);
            EnrichWithContext(writer);
            EnrichWithHostname(writer);
        }

        private void SetTimestamp(MetricEventWriter writer)
        {
            writer.SetTimestamp(DateTimeOffset.UtcNow);
        }

        private void EnrichWithContext(IMetricEventWriter writer)
        {
            foreach (var pair in Context.Properties.Current)
            {
                if (configuration.ContextFieldsWhitelist.Contains(pair.Key))
                    writer.SetTag(pair.Key, Convert.ToString(pair.Value));
            }
        }

        private void EnrichWithHostname(IMetricEventWriter writer)
        {
            writer.SetTag("host", HostnameProvider.Get());
        }
    }
}