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

            spanPool = new UnlimitedLazyPool<Span>(
                () => new Span
                {
                    Annotations = new Dictionary<string, string>()
                });
        }

        public static ISpanBuilder BeginSpan()
        {
            var isEnabled = Configuration.IsEnabled();
            var airlock = Configuration.AirlockClient;
            var airlockRoutingKey = configuration.AirlockRoutingKey?.Invoke();

            if (!isEnabled || airlock == null || airlockRoutingKey == null)
                return new FakeSpanBuilder();

            var pooledSpan = spanPool.AcquireHandle();
            var contextScope = TraceContextScope.Begin();

            return new SpanBuilder(airlockRoutingKey, airlock, pooledSpan, contextScope, configuration);
        }

        public static ITraceConfiguration Configuration => configuration;
    }
}
