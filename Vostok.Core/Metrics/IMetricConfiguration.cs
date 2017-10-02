using System.Collections.Generic;
using Vostok.Airlock;

namespace Vostok.Metrics
{
    public interface IMetricConfiguration
    {
        IAirlock Airlock { get; set; }
        string EventRoutingKey { get; set; }
        string MetricRoutingKey { get; set; }
        ISet<string> ContextFieldsWhitelist { get; }
    }
}