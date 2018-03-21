using System;

namespace Vostok.Tracing
{
    public interface ISpanBuilder : IDisposable
    {
        bool IsCanceled { get; set; }
        bool IsEndless { get; set; }

        void SetAnnotation<TValue>(string key, TValue value);
        void SetBeginTimestamp(DateTimeOffset timestamp);
        void SetEndTimestamp(DateTimeOffset timestamp);
    }
}
