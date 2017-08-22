namespace Vostok.Tracing
{
    public static class Trace
    {
        public static ISpanBuilder BeginSpan(string operationName)
        {
            var isEnabled = Configuration.IsEnabled();
            var airlock = Configuration.Airlock;

            if (!isEnabled || airlock == null)
                return new FakeSpanBuilder();

            return new SpanBuilder(operationName);
        }

        public static ITraceConfiguration Configuration { get; } = new TraceConfiguration();
    }
}
