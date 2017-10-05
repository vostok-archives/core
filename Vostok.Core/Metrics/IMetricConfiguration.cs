using System.Collections.Generic;

namespace Vostok.Metrics
{
    public interface IMetricConfiguration
    {
        IMetricEventReporter Reporter { get; set; }
        ISet<string> ContextFieldsWhitelist { get; }
    }
}