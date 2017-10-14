using System;
using System.IO;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Model;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class RecordSerializer : IRecordSerializer
    {
        private readonly DataSize maximumRecordSize;
        private readonly ILog log;

        public RecordSerializer(DataSize maximumRecordSize, ILog log)
        {
            this.maximumRecordSize = maximumRecordSize;
            this.log = log;
        }

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

                var recordSize = positionAfterSerialization - positionBeforeSerialization;
                if (recordSize > maximumRecordSize.Bytes)
                {
                    LogDiscardingLargeRecord(recordSize);
                    return false;
                }

                writer.Position = payloadLengthPosition;

                writer.Write(recordSize);

                writer.Position = positionAfterSerialization;

                return true;
            }
            catch (InternalBufferOverflowException)
            {
                return false;
            }
        }

        private void LogDiscardingLargeRecord(int recordSize)
        {
            log.Warn($"Discarded gigantic record with size = {recordSize.Bytes()} larger than max allowed size = {maximumRecordSize}.");
        }
    }
}
