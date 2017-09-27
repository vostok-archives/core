using System;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    public class NameValueConverter : IMetricConverter<long>, IMetricConverter<double>
    {
        private readonly string name;

        public NameValueConverter(string name)
        {
            this.name = name;
        }

        public IEnumerable<Metric> Convert(long metric, DateTimeOffset timestamp)
        {
            return Convert((double) metric, timestamp);
        }

        public IEnumerable<Metric> Convert(double metric, DateTimeOffset timestamp)
        {
            yield return new Metric {Name = name, Value = metric, Timestamp = timestamp};
        }
    }
}