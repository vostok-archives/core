using System.Collections.Generic;

namespace Vostok.Metrics
{
    public interface IMetricConfiguration
    {
        /// <summary>
        /// TODO create xmldoc for this
        /// </summary>
        ISet<string> ContextFieldsWhitelist { get; }

        IMetricEventReporter Reporter { get; set; }
    }
}