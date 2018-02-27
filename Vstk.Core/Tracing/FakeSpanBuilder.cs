using System;

namespace Vstk.Tracing
{
    internal class FakeSpanBuilder : ISpanBuilder
    {
        public bool IsCanceled { get; set; } = true;
        public bool IsEndless { get; set; } = false;

        public void SetAnnotation<TValue>(string key, TValue value)
        {
        }

        public void SetBeginTimestamp(DateTimeOffset timestamp)
        {
        }

        public void SetEndTimestamp(DateTimeOffset timestamp)
        {
        }

        public void Dispose()
        {
        }
    }
}
