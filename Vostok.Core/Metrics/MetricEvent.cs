using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    public class MetricEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        public IReadOnlyDictionary<string, string> Tags { get; set; }
        public IReadOnlyDictionary<string, double> Values { get; set; }
    }
}