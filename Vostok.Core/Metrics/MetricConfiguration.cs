using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Metrics
{
    internal class MetricConfiguration : IMetricConfiguration
    {
        public IMetricEventReporter Reporter { get; set; }
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);
    }
}