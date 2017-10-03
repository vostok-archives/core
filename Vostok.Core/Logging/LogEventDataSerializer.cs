using System;
using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Logging
{
    public class LogEventDataSerializer : IAirlockSerializer<LogEventData>, IAirlockDeserializer<LogEventData>
    {
        public LogEventData Deserialize(IAirlockSource source)
        {
            var reader = source.Reader;

            return new LogEventData
            {
                Timestamp = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero),
                LogLevel = reader.ReadString(),
                MessageTemplate = reader.ReadString(),
                Exception = reader.ReadString(),
                Properties = reader.ReadDictionary(r => r.ReadString(), r => r.ReadString())
            };
        }

        public void Serialize(LogEventData item, IAirlockSink sink)
        {
            var writer = sink.Writer;

            writer.Write(item.Timestamp.UtcTicks);
            writer.Write(item.LogLevel);
            writer.Write(item.MessageTemplate);
            writer.Write(item.Exception);
            writer.WriteDictionary(item.Properties, (w, s) => w.Write(s), (w, o) => w.Write(o));
        }
    }
}