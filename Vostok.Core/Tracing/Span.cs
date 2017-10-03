using System;
using System.Collections.Generic;

namespace Vostok.Tracing
{
    public class Span
    {
        public Guid TraceId { get; set; }
        public Guid SpanId { get; set; }
        public Guid? ParentSpanId { get; set; }
        public DateTimeOffset BeginTimestamp { get; set; }
        public DateTimeOffset? EndTimestamp { get; set; }
        public IDictionary<string, string> Annotations { get; set; }
    }
}
