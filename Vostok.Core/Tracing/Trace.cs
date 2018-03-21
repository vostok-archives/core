using System.Collections.Generic;
using Vstk.Commons.Collections;

namespace Vstk.Tracing
{
    public static class Trace
    {
        private static readonly IPool<Span> spanPool;
        private static readonly ITraceConfiguration configuration;

        static Trace()
        {
            configuration = new TraceConfiguration();

            spanPool = new UnlimitedLazyPool<Span>(
                () => new Span
                {
                    Annotations = new Dictionary<string, string>()
                });
        }

        public static ISpanBuilder BeginSpan()
        {
            var traceReporter = Configuration.Reporter;
            
            if (traceReporter == null)
                return new FakeSpanBuilder();

            var pooledSpan = spanPool.AcquireHandle();
            var contextScope = TraceContextScope.Begin();

            return new SpanBuilder(traceReporter, pooledSpan, contextScope, configuration);
        }

        public static ITraceConfiguration Configuration => configuration;
    }
}
