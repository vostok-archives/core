using System;
using System.Collections.Generic;
using Vstk.Commons.Collections;

namespace Vstk.Tracing
{
    internal class TraceConfiguration : ITraceConfiguration
    {
        public ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

        public ISet<string> InheritedFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

        public ITraceReporter Reporter { get; set; }
    }
}
