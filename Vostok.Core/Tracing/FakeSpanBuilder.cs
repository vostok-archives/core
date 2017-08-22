using System;

namespace Vostok.Tracing
{
    internal class FakeSpanBuilder : ISpanBuilder
    {
        public bool IsCanceled => true;

        public void SetAnnotation<TValue>(string key, TValue value)
        {
        }

        public void SetBeginTimestamp(DateTimeOffset timestamp)
        {
        }

        public void SetEndTimestamp(DateTimeOffset timestamp)
        {
        }

        public void MakeEndless()
        {
        }

        public void Cancel()
        {
        }
    }
}
