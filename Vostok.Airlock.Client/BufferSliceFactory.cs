using System;
using System.Collections.Generic;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class BufferSliceFactory : IBufferSliceFactory
    {
        // (iloktionov): Работает в предположении о том, что размер одной записи в буфере не может превышать maximumSliceLength.
        public IEnumerable<BufferSlice> Cut(IBuffer buffer, int maximumSliceLength)
        {
            if (buffer.SnapshotLength <= maximumSliceLength)
            {
                yield return new BufferSlice(buffer, 0, buffer.SnapshotLength);
                yield break;
            }

            var currentOffset = 0;
            var currentSize = 0;
            var reader = new BinaryBufferReader(buffer.InternalBuffer, 0);

            for (var i = 0; i < buffer.SnapshotCount; i++)
            {
                var recordLength = ReadRecordLength(reader);
                if (recordLength > maximumSliceLength)
                    throw new InvalidOperationException($"Bug! Encountered a record with length = {recordLength} > max slice size {maximumSliceLength}.");

                if (currentSize + recordLength > maximumSliceLength)
                {
                    yield return new BufferSlice(buffer, currentOffset, currentSize);
                    currentOffset += currentSize;
                    currentSize = 0;
                }

                currentSize += recordLength;
            }

            if (currentSize > 0)
                yield return new BufferSlice(buffer, currentOffset, currentSize);
        }

        private static int ReadRecordLength(IBinaryReader reader)
        {
            reader.Position += sizeof(long);

            var recordLength = sizeof(long) + reader.ReadInt32();

            reader.Position += recordLength;

            return recordLength;
        }
    }
}