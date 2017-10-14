using System.Collections.Generic;
using Vostok.Configuration;

namespace Vostok.Metrics
{
    internal class DefaultMetricConfiguration : IMetricConfiguration
    {
        public IMetricEventReporter Reporter => VostokConfiguration.Metrics.Reporter;
        public ISet<string> ContextFieldsWhitelist => VostokConfiguration.Metrics.ContextFieldsWhitelist;
    }
}