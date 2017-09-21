using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    public class AirlockMessageSerializer : IAirlockSerializer<AirlockMessage>
    {
        public void Serialize(AirlockMessage item, IAirlockSink sink)
        {
            var writer = sink.Writer;
            writer.Write(item.Version);
            writer.WriteCollection(item.EventGroups,WriteGroup);
        }

        private static void WriteGroup(IBinaryWriter writer, EventGroup g)
        {
            writer.Write(g.EventType);
            writer.WriteCollection(g.EventRecords, WriteRecord);
        }

        private static void WriteRecord(IBinaryWriter writer, EventRecord r)
        {
            writer.Write(r.Timestamp);
            writer.Write(r.Data);
        }
    }
}