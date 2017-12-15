using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    internal class MetricScopeTagEnricher : IMetricScope
    {
        private readonly IMetricScope parent;
        private readonly IReadOnlyDictionary<string, string> tags;

        public MetricScopeTagEnricher(IMetricScope parent, IReadOnlyDictionary<string, string> tags)
        {
            this.parent = parent;
            this.tags = tags;
            DefaultInterval = parent.DefaultInterval;
        }

        public IMetricEventWriter WriteEvent()
        {
            return Enrich(parent.WriteEvent());
        }

        public IMetricEventWriter WriteMetric()
        {
            return Enrich(parent.WriteMetric());
        }

        public TimeSpan DefaultInterval { get; }

        private IMetricEventWriter Enrich(IMetricEventWriter writer)
        {
            foreach (var kvp in tags)
                writer.SetTag(kvp.Key, kvp.Value);
            return writer;
        }
    }
}