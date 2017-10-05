using System;
using System.Collections.Generic;
using Vostok.Airlock;

namespace Vostok.Tracing
{
    public interface ITraceConfiguration
    {
        ISet<string> ContextFieldsWhitelist { get; }

        ISet<string> InheritedFieldsWhitelist { get; }

        Func<bool> IsEnabled { get; set; }

        Func<string> AirlockRoutingKey { get; set; }

        // TODO(iloktionov): Invent a way to automatically fill this with an out-of-the-box implementation in apps.
        IAirlockClient AirlockClient { get; set; }
    }
}
