using System.Collections.Generic;

namespace Vostok.Metrics
{
    public interface IMetricConfiguration
    {
        IMetricEventReporter Reporter { get; }
        ISet<string> ContextFieldsWhitelist { get; }
    }
}