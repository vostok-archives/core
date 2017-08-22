using System;
using System.Collections.Generic;
using Vostok.Airlock;

namespace Vostok.Tracing
{
    public interface ITraceConfiguration
    {
        ISet<string> ContextFieldsWhitelist { get; }

        Func<bool> IsEnabled { get; set; }

        Func<string> AirlockRoutingKey { get; set; }

        IAirlock Airlock { get; set; }
    }
}