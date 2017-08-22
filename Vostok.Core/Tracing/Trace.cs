using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Tracing
{
    public static class Trace
    {
        private static readonly IPool<Span> spanPool;
        private static readonly TraceConfiguration configuration;

        static Trace()
        {
            configuration = new TraceConfiguration();

            spanPool = new UnlimitedLazyPool<Span>(() => new Span
            {
                Annotations = new Dictionary<string, string>()
            });
        }

        public static ISpanBuilder BeginSpan(string operationName)
        {
            var isEnabled = Configuration.IsEnabled();
            var airlock = Configuration.Airlock;

            if (!isEnabled || airlock == null)
                return new FakeSpanBuilder();

            return new SpanBuilder(operationName);
        }

        public static ITraceConfiguration Configuration => configuration;
    }
}
