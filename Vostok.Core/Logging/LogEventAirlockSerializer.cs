using Vostok.Airlock;
using Vostok.Commons.Binary;

namespace Vostok.Logging
{
    public class LogEventAirlockSerializer : IAirlockSerializer<LogEvent>
    {
        public void Serialize(LogEvent item, IAirlockSink sink)
        {
            var writer = sink.Writer;
            writer.Write((int)item.Level);
            writer.Write(string.Format(item.MessageTemplate, item.MessageParameters));
            writer.Write(item.Exception.ToString());
            writer.WriteDictionary(item.Properties, (w, s) => w.Write(s), (w, o) => w.Write(o.ToString()));
        }
    }
}