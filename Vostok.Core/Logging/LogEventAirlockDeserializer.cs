using System.Text;
using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Logging
{
    public class LogEventAirlockDeserializer : IAirlockDeserializer<LogEventData>
    {
        public LogEventData Deserialize(IAirlockDeserializationSink sink)
        {
            var reader = sink.Reader;
            var logEventData = new LogEventData
            {
                Level = (LogLevel) reader.ReadInt32(),
                Message = reader.ReadString(),
                Exception = reader.ReadString(),
                Properties = reader.ReadDictionary(r => r.ReadString(), r => r.ReadString())
            };
            return logEventData;
        }
    }
}