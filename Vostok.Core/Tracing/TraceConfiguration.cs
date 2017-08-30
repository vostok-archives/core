using System;
using System.Collections.Generic;
using Vostok.Airlock;
using Vostok.Commons.Collections;

namespace Vostok.Tracing
{
    internal class TraceConfiguration : ITraceConfiguration
    {
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

        public Func<bool> IsEnabled { get; set; } = () => true;

        public Func<string> AirlockRoutingKey { get; set; } = () => "tracing";

        public IAirlock Airlock { get; set; }
    }
}
