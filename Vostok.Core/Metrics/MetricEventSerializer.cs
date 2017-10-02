using System;
using System.Collections.Generic;
using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Metrics
{
    internal class MetricEventSerializer : 
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

        public MetricEvent Deserialize(IAirlockDeserializationSink sink)
        {
            var metricEvent = new MetricEvent();

            metricEvent.Timestamp = new DateTimeOffset(sink.Reader.ReadInt64(), TimeSpan.Zero);

            var count = sink.Reader.ReadInt32();
            metricEvent.Tags = new Dictionary<string, string>(count);
            for (var i = 0; i < count; i++)
            {
                var key = sink.Reader.ReadString();
                var value = sink.Reader.ReadString();
                metricEvent.Tags[key] = value;
            }

            count = sink.Reader.ReadInt32();
            metricEvent.Values = new Dictionary<string, double>(count);
            for (var i = 0; i < count; i++)
            {
                var key = sink.Reader.ReadString();
                var value = sink.Reader.ReadDouble();
                metricEvent.Values[key] = value;
            }

            return metricEvent;
        }
    }
}