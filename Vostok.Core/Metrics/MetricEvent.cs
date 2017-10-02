using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    internal class MetricEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public Dictionary<string, double> Values { get; set; }
    }
}