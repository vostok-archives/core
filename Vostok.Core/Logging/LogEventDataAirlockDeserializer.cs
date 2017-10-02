using System.Text;
using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Logging
{
    public class LogEventDataAirlockDeserializer : IAirlockDeserializer<LogEventData>
    {
        public LogEventData Deserialize(IAirlockSource source)
        {
            var reader = source.Reader;
            var logEventData = new LogEventData
            {
                Timestamp = reader.ReadInt64(),
                Properties = reader.ReadDictionary(r => r.ReadString(), r => r.ReadString())
            };
            return logEventData;
        }
    }
}