using System;
using System.Collections.Generic;
using Vstk.Commons.Collections;

namespace Vstk.Metrics
{
    public class MetricConfiguration : IMetricConfiguration
    {
        public IMetricEventReporter Reporter { get; set; }
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);
    }
}