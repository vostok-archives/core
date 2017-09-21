using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Logging
{
    public class LogEventDataAirlockSerializer : IAirlockSerializer<LogEventData>
    {
        public void Serialize(LogEventData item, IAirlockSink sink)
        {
            var writer = sink.Writer;
            writer.Write(item.Timestamp);
            writer.WriteDictionary(item.Properties, (w, s) => w.Write(s), (w, o) => w.Write(o.ToString()));
        }
    }
}