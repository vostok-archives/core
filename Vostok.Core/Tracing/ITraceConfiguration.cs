using System;
using System.Collections.Generic;
using Vostok.Flow;

namespace Vostok.Tracing
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

        Func<bool> IsEnabled { get; set; }

        // TODO(iloktionov): Invent a way to automatically fill this with an out-of-the-box implementation in apps.
        ITraceReporter Reporter { get; set; }
    }
}
