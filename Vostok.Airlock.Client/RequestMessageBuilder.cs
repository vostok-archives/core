using System;
using System.Text;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class RequestMessageBuilder
    {
        private readonly BinaryBufferWriter writer;
        private int groupsWritten;

        public RequestMessageBuilder(byte[] buffer)
        {
            writer = new BinaryBufferWriter(buffer);

            WriteVersion();
            WriteGroupsCount();
        }

        public ArraySegment<byte> Message => writer.FilledSegment;

        public bool TryAdd(string routingKey, IBuffer buffer)
        {
            if (!Fits(routingKey, buffer))
                return false;

            WriteRecordsGroup(routingKey, buffer);

            groupsWritten++;

            WriteGroupsCount();

            return true;
        }

        private bool Fits(string routingKey, IBuffer buffer)
        {
            var requiredSpace = buffer.Position + sizeof (int) + sizeof (int) + Encoding.UTF8.GetMaxByteCount(routingKey.Length);
            var remainingSpace = writer.Buffer.Length - writer.Position;

            return requiredSpace <= remainingSpace;
        }

        private void WriteVersion()
        {
            writer.Write((short) 1);
        }

        private void WriteGroupsCount()
        {
            var positionBefore = writer.Position;

            writer.Position = 2;
            writer.Write(groupsWritten);

            if (positionBefore > 2)
                writer.Position = positionBefore;
        }

        private void WriteRecordsGroup(string routingKey, IBuffer buffer)
        {
            writer.Write(routingKey);
            writer.Write(buffer.WrittenRecords);
            writer.WriteWithoutLengthPrefix(buffer.InternalBuffer, 0, buffer.Position);
        }
    }
}
