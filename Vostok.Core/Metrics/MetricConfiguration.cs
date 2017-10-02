using System;
using System.Collections.Generic;
using Vostok.Airlock;
using Vostok.Commons.Collections;

namespace Vostok.Metrics
{
    public class MetricConfiguration : IMetricConfiguration
    {
        public IAirlock Airlock { get; set; }
        public string EventRoutingKey { get; set; } = "metric_events";
        public string MetricRoutingKey { get; set; } = "metrics";
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);
    }
}