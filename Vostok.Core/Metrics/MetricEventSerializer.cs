using System;
using System.Collections.Generic;
using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Metrics
{
    public class MetricEventSerializer :
        IAirlockSerializer<MetricEvent>,
        IAirlockDeserializer<MetricEvent>
    {
        public void Serialize(MetricEvent item, IAirlockSink sink)
        {
            sink.Writer.Write(item.Timestamp.Ticks);

            sink.Writer.Write(item.Tags.Count);
            foreach (var kvp in item.Tags)
            {
                sink.Writer.Write(kvp.Key);
                sink.Writer.Write(kvp.Value);
            }

            sink.Writer.Write(item.Values.Count);
            foreach (var kvp in item.Values)
            {
                sink.Writer.Write(kvp.Key);
                sink.Writer.Write(kvp.Value);
            }
        }

        public MetricEvent Deserialize(IAirlockSource source)
        {
            var timestamp = new DateTimeOffset(source.Reader.ReadInt64(), TimeSpan.Zero);

            var count = source.Reader.ReadInt32();
            var tags = new Dictionary<string, string>(count);
            for (var i = 0; i < count; i++)
            {
                var key = source.Reader.ReadString();
                var value = source.Reader.ReadString();
                tags[key] = value;
            }

            count = source.Reader.ReadInt32();
            var values = new Dictionary<string, double>(count);
            for (var i = 0; i < count; i++)
            {
                var key = source.Reader.ReadString();
                var value = source.Reader.ReadDouble();
                values[key] = value;
            }

            return new MetricEvent
            {
                Timestamp = timestamp,
                Tags = tags,
                Values = values
            };
        }
    }
}