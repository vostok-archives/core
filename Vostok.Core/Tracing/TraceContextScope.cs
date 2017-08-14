using System;

namespace Vostok.Tracing
{
    internal class TraceContextScope : IDisposable
    {
        public TraceContextScope(TraceContext newContext)
        {
            Parent = TraceContext.Current;
            
            TraceContext.Current = Current = newContext;
        }

        public TraceContext Current { get; }
        public TraceContext Parent { get; }

        public void Dispose()
        {
            TraceContext.Current = Parent;
        }
    }
}
