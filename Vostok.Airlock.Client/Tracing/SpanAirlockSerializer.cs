using System;
using System.IO;
using Vostok.Commons.Binary;
using Vostok.Tracing;

namespace Vostok.Airlock.Tracing
{
    public class SpanAirlockSerializer : IAirlockSerializer<Span>, IAirlockDeserializer<Span>
    {
        private const byte formatVersion = 1;

        public void Serialize(Span span, IAirlockSink sink)
        {
            var writer = sink.Writer;

            writer.Write(formatVersion);
            writer.Write(span.TraceId);
            writer.Write(span.SpanId);
            writer.WriteNullable(span.ParentSpanId, (w, id) => w.Write(id));
            writer.Write(span.BeginTimestamp.UtcTicks);
            writer.WriteNullable(span.EndTimestamp, (w, t) => w.Write(t.UtcTicks));
            writer.WriteDictionary(span.Annotations, (w, key) => w.Write(key), (w, value) => w.Write(value));
        }

        public Span Deserialize(IAirlockSource source)
        {
            var reader = source.Reader;
            var version = reader.ReadByte();
            if (version != formatVersion)
                throw new InvalidDataException("invalid format version: " + version);

            return new Span
            {
                TraceId = reader.ReadGuid(),
                SpanId = reader.ReadGuid(),
                ParentSpanId = reader.ReadNullableStruct(x => x.ReadGuid()),
                BeginTimestamp = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero),
                EndTimestamp = reader.ReadNullableStruct(x => new DateTimeOffset(x.ReadInt64(), TimeSpan.Zero)),
                Annotations = reader.ReadDictionary(r => r.ReadString(), r => r.ReadString())
            };
        }
    }
}
