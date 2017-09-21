using System.Collections.Generic;

namespace Vostok.Airlock
{
    public class EventRecord
    {
        public long Timestamp { get; set; }
        public byte[] Data { get; set; }
    }

    public class EventGroup
    {
        public short EventType { get; set; }
        public List<EventRecord> EventRecords { get; set; }
    }

    public class AirlockMessage
    {
        public short Version { get; } = 1;
        public List<EventGroup> EventGroups { get; set; }
    }
}