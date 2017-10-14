using System;
using System.Text;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class RequestMessageBuilder
    {
        public const int CommonHeaderSize = sizeof (short) + sizeof (int);

        public static int EstimateEventGroupHeaderSize(string routingKey)
        {
            return sizeof (int) + sizeof (int) + Encoding.UTF8.GetMaxByteCount(routingKey.Length);
        }

        private readonly BinaryBufferWriter writer;
        private int groupsWritten;

        public RequestMessageBuilder(byte[] buffer)
        {
            writer = new BinaryBufferWriter(buffer);

            WriteVersion();
            WriteGroupsCount();
        }

        public ArraySegment<byte> Message => writer.FilledSegment;

        public bool TryAppend(string routingKey, BufferSlice content)
        {
            if (!Fits(routingKey, content))
                return false;

            WriteRecordsGroup(routingKey, content);

            groupsWritten++;

            WriteGroupsCount();

            return true;
        }

        private bool Fits(string routingKey, BufferSlice content)
        {
            var requiredSpace = content.Length + EstimateEventGroupHeaderSize(routingKey);
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

        private void WriteRecordsGroup(string routingKey, BufferSlice content)
        {
            writer.Write(routingKey);
            writer.Write(content.Items);
            writer.WriteWithoutLengthPrefix(content.Buffer.InternalBuffer, content.Offset, content.Length);
        }
    }
}
