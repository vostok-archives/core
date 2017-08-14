using System;

namespace Vostok.Tracing
{
    public interface ISpanBuilder
    {
        bool IsCanceled { get; }

        void SetAnnotation<TValue>(string key, TValue value);
        void SetBeginTimestamp(DateTimeOffset timestamp);
        void SetEndTimestamp(DateTimeOffset timestamp);

        void MakeEndless();
        void Cancel();
    }
}
