using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;
using Vostok.Commons.Extensions.UnitConvertions;

namespace Vostok.Metrics
{
    public class MetricConfiguration : IMetricConfiguration
    {
        public IMetricEventReporter Reporter { get; set; }
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

        public TimeSpan DefaultInterval { get; set; } = 1.Minutes();
    }
}