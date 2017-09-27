using System;

namespace Vostok.Metrics
{
    public class Metric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}