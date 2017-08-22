using System;

namespace Vostok.Tracing
{
    internal class TraceContextScope : IDisposable
    {
        public static TraceContextScope Begin()
        {
            var oldContext = TraceContext.Current;
            var newContext = new TraceContext(oldContext?.TraceId ?? Guid.NewGuid(), Guid.NewGuid());

            TraceContext.Current = newContext;

            return new TraceContextScope(newContext, oldContext);
        }

        public static TraceContextScope Begin(TraceContext newContext)
        {
            var oldContext = TraceContext.Current;

            TraceContext.Current = newContext;

            return new TraceContextScope(newContext, oldContext);
        }

        private TraceContextScope(TraceContext current, TraceContext parent)
        {
            Current = current;
            Parent = parent;
        }

        public TraceContext Current { get; }
        public TraceContext Parent { get; }

        public void Dispose()
        {
            TraceContext.Current = Parent;
        }
    }
}
