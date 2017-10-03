using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Tracing
{
    internal class SpanAirlockSerializer : IAirlockSerializer<Span>
    {
        private const byte FormatVersion = 1;

        public void Serialize(Span span, IAirlockSink sink)
        {
            var writer = sink.Writer;

            writer.Write(FormatVersion);
            writer.Write(span.TraceId);
            writer.Write(span.SpanId);
            writer.WriteNullable(span.ParentSpanId, (w, id) => w.Write(id));
            writer.Write(span.BeginTimestamp.UtcTicks);
            writer.WriteNullable(span.EndTimestamp, (w, t) => w.Write(t.UtcTicks));
            writer.WriteDictionary(span.Annotations, (w, key) => w.Write(key), (w, value) => w.Write(value));
        }
    }
}
