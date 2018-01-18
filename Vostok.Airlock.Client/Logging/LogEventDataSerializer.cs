using System;
using System.IO;
using Vostok.Commons.Binary;
using Vostok.Logging;

namespace Vostok.Airlock.Logging
{
    public class LogEventDataSerializer : IAirlockSerializer<LogEventData>, IAirlockDeserializer<LogEventData>
    {
        private const byte formatVersion = 1;

        public LogEventData Deserialize(IAirlockSource source)
        {
            var reader = source.Reader;
            var version = reader.ReadByte();
            if (version != formatVersion)
                throw new InvalidDataException("invalid format version: " + version);

            return new LogEventData
            {
                Timestamp = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero),
                Level = (LogLevel)reader.ReadInt32(),
                Message = reader.ReadNullable(r => r.ReadString()),
                Exceptions = reader.ReadNullable(x => x.ReadList(ReadException)),
                Properties = reader.ReadNullable(x => x.ReadDictionary(y => y.ReadString(), y => y.ReadString()))
            };
        }

        public void Serialize(LogEventData item, IAirlockSink sink)
        {
            var writer = sink.Writer;

            writer.Write(formatVersion);
            writer.Write(item.Timestamp.UtcTicks);
            writer.Write((int)item.Level);
            writer.WriteNullable(item.Message, (w, s) => w.Write(s));
            writer.WriteNullable(item.Exceptions, (a, b) => a.WriteCollection(b, WriteException));
            writer.WriteNullable(item.Properties, (a, b) => a.WriteDictionary(b, (c, d) => c.Write(d), (c, d) => c.Write(d)));
        }

        private static LogEventException ReadException(IBinaryReader reader)
        {
            return new LogEventException
            {
                Message = reader.ReadNullable(r => r.ReadString()),
                Type = reader.ReadNullable(r => r.ReadString()),
                Module = reader.ReadNullable(r => r.ReadString()),
                Stack = reader.ReadNullable(r => r.ReadList(ReadStackFrame))
            };
        }

        private static void WriteException(IBinaryWriter writer, LogEventException logEventException)
        {
            writer.WriteNullable(logEventException.Message, (w, s) => w.Write(s));
            writer.WriteNullable(logEventException.Type, (w, s) => w.Write(s));
            writer.WriteNullable(logEventException.Module, (w, s) => w.Write(s));
            writer.WriteNullable(logEventException.Stack, (w, x) => w.WriteCollection(x, WriteStackFrame));
        }

        private static LogEventStackFrame ReadStackFrame(IBinaryReader reader)
        {
            return new LogEventStackFrame
            {
                Module = reader.ReadNullable(r => r.ReadString()),
                Function = reader.ReadNullable(r => r.ReadString()),
                Source = reader.ReadNullable(r => r.ReadString()),
                Filename = reader.ReadNullable(r => r.ReadString()),
                LineNumber = reader.ReadInt32(),
                ColumnNumber = reader.ReadInt32()
            };
        }

        private static void WriteStackFrame(IBinaryWriter writer, LogEventStackFrame stackFrame)
        {
            writer.WriteNullable(stackFrame.Module, (w, s) => w.Write(s));
            writer.WriteNullable(stackFrame.Function, (w, s) => w.Write(s));
            writer.WriteNullable(stackFrame.Source, (w, s) => w.Write(s));
            writer.WriteNullable(stackFrame.Filename, (w, s) => w.Write(s));
            writer.Write(stackFrame.LineNumber);
            writer.Write(stackFrame.ColumnNumber);
        }
    }
}