using System;

namespace Vostok.Tracing
{
    internal class SpanBuilder : ISpanBuilder
    {
        private readonly string operationName;

        public SpanBuilder(string operationName)
        {
            this.operationName = operationName;
        }

        public bool IsCanceled { get; private set; }

        public void SetAnnotation<TValue>(string key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void SetBeginTimestamp(DateTimeOffset timestamp)
        {
            throw new NotImplementedException();
        }

        public void SetEndTimestamp(DateTimeOffset timestamp)
        {
            throw new NotImplementedException();
        }

        public void MakeEndless()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
