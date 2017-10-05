using System;
using System.IO;
using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Logging.Airlock
{
    public class LogEventDataSerializer : IAirlockSerializer<LogEventData>, IAirlockDeserializer<LogEventData>
    {
        private const byte FormatVersion = 1;

        public LogEventData Deserialize(IAirlockSource source)
        {
            var reader = source.Reader;
            var version = reader.ReadByte();
            if (version != FormatVersion)
                throw new InvalidDataException("invalid format version: " + version);

            return new LogEventData
            {
                Timestamp = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero),
                Level = (LogLevel) reader.ReadInt32(),
                Message = reader.ReadNullable(r => r.ReadString()),
                Exception = reader.ReadNullable(r => r.ReadString()),
                Properties = reader.ReadNullable(x => x.ReadDictionary(y => y.ReadString(), y => y.ReadString()))
            };
        }

        public void Serialize(LogEventData item, IAirlockSink sink)
        {
            var writer = sink.Writer;

            writer.Write(FormatVersion);
            writer.Write(item.Timestamp.UtcTicks);
            writer.Write((int) item.Level);
            writer.WriteNullable(item.Message, (w, s) => w.Write(s));
            writer.WriteNullable(item.Exception, (w, s) => w.Write(s));
            writer.WriteNullable(item.Properties, (a, b) => a.WriteDictionary(b, (c, d) => c.Write(d), (c, d) => c.Write(d)));
        }
    }
}