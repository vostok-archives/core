using System;
using System.Collections.Generic;

namespace Vostok.Tracing
{
    public interface ITraceConfiguration
    {
        /// <summary>
        /// TODO create xmldoc for this
        /// </summary>
        ISet<string> ContextFieldsWhitelist { get; }

        /// <summary>
        /// TODO create xmldoc for this
        /// </summary>
        ISet<string> InheritedFieldsWhitelist { get; }

        Func<bool> IsEnabled { get; set; }

        ITraceReporter Reporter { get; set; }
    }
}
