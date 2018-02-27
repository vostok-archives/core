using System.Collections.Generic;
using Vstk.Flow;

namespace Vstk.Tracing
{
    public interface ITraceConfiguration
    {
        /// <summary>
        /// Fields to be added as trace annotations from current <see cref="Context"/>
        /// </summary>
        ISet<string> ContextFieldsWhitelist { get; }

        /// <summary>
        /// Fields to be added as trace annotations from parent span
        /// </summary>
        ISet<string> InheritedFieldsWhitelist { get; }

        ITraceReporter Reporter { get; set; }
    }
}
