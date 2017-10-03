using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    public class MetricEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public Dictionary<string, double> Values { get; set; }
    }
}