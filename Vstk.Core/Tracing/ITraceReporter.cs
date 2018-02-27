namespace Vstk.Tracing
{
    public interface ITraceReporter
    {
        void SendSpan(Span span);
    }
}