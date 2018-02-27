using System.Collections.Generic;
using Vstk.Flow;

namespace Vstk.Metrics
{
    public interface IMetricConfiguration
    {
        /// <summary>
        /// Fields to be added as tags from current <see cref="Context"/>
        /// </summary>
        ISet<string> ContextFieldsWhitelist { get; }

        IMetricEventReporter Reporter { get; set; }
    }
}