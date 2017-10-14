using System;
using System.Collections.Generic;

namespace Vostok.Tracing
{
    public interface ITraceConfiguration
    {
        ISet<string> ContextFieldsWhitelist { get; }

        ISet<string> InheritedFieldsWhitelist { get; }

        Func<bool> IsEnabled { get; set; }

        ITraceReporter Reporter { get; set; }
    }
}
