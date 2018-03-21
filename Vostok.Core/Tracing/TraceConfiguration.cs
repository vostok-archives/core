using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Tracing
{
    internal class TraceConfiguration : ITraceConfiguration
    {
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

        public ISet<string> InheritedFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

        public ITraceReporter Reporter { get; set; }
    }
}
