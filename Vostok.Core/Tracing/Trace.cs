namespace Vostok.Tracing
{
    public static class Trace
    {
        public static ISpanBuilder BeginSpan(string operationName)
        {
            return new SpanBuilder(operationName);
        }

        public static ITraceConfiguration Configuration { get; }
    }
}
