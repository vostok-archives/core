using System;

namespace Vostok.Airlock
{
    internal class RecordWriter : IRecordWriter
    {
        private readonly IRecordSerializer recordSerializer;

        public RecordWriter(IRecordSerializer recordSerializer)
        {
            this.recordSerializer = recordSerializer;
        }

        public bool TryWrite<T>(T item, IAirlockSerializer<T> serializer, DateTimeOffset timestamp, IBufferPool bufferPool)
        {
            if (!bufferPool.TryAcquire(out var buffer))
                return false;

            var startingPosition = buffer.Position;
            var serializationSucceeded = false;

            try
            {
                serializationSucceeded = recordSerializer.TrySerialize(item, serializer, timestamp, buffer);
            }
            finally
            {
                if (serializationSucceeded)
                {
                    buffer.WrittenRecords++;
                }
                else
                {
                    buffer.Position = startingPosition;
                }
                
                bufferPool.Release(buffer);
            }

            return serializationSucceeded;
        }
    }
}