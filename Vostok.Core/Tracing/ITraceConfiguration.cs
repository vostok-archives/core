using System;
using System.Collections.Generic;

namespace Vostok.Tracing
{
    public interface ITraceConfiguration
    {
        ISet<string> ContextFieldsWhitelist { get; }

        ISet<string> InheritedFieldsWhitelist { get; }

        Func<bool> IsEnabled { get; set; }

        // TODO(iloktionov): Invent a way to automatically fill this with an out-of-the-box implementation in apps.
        ITraceReporter Reporter { get; set; }
    }
}
