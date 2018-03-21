namespace Vostok.Tracing
{
    public interface ITraceReporter
    {
        void SendSpan(Span span);
    }
}