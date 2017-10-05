using System;
using System.IO;

namespace Vostok.Airlock
{
    internal class RecordSerializer : IRecordSerializer
    {
        // TODO(iloktionov): disallow very large items
        public bool TrySerialize<T>(T item, IAirlockSerializer<T> serializer, DateTimeOffset timestamp, IBuffer buffer)
        {
            try
            {
                var unixTimestamp = timestamp.ToUniversalTime().ToUnixTimeMilliseconds();
                var writer = buffer.Writer;

                writer.Write(unixTimestamp);

                var payloadLengthPosition = writer.Position;

                writer.Write(0);

                var positionBeforeSerialization = writer.Position;

                serializer.Serialize(item, buffer);

                var positionAfterSerialization = writer.Position;

                writer.Position = payloadLengthPosition;

                writer.Write(positionAfterSerialization - positionBeforeSerialization);

                writer.Position = positionAfterSerialization;

                return true;
            }
            catch (InternalBufferOverflowException)
            {
                return false;
            }
        }
    }
}