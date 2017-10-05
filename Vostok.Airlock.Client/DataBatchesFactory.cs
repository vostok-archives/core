using System.Collections.Generic;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataBatchesFactory : IDataBatchesFactory
    {
        private readonly IReadOnlyDictionary<string, IBufferPool> bufferPools;
        private readonly byte[] messageBuffer;
        private readonly ILog log;

        public DataBatchesFactory(IReadOnlyDictionary<string, IBufferPool> bufferPools, byte[] messageBuffer, ILog log)
        {
            this.bufferPools = bufferPools;
            this.messageBuffer = messageBuffer;
            this.log = log;
        }

        public IEnumerable<IDataBatch> CreateBatches()
        {
            var context = new DataBatchBuildingContext(messageBuffer);

            foreach ((var routingKey, var buffer) in EnumerateAllBuffers())
            {
                foreach (var batch in HandleBuffer(routingKey, buffer, context))
                {
                    yield return batch;
                }
            }

            if (!context.IsEmpty)
                yield return context.CreateBatch();
        }

        private IEnumerable<(string, IBuffer)> EnumerateAllBuffers()
        {
            foreach (var pair in bufferPools)
            {
                var routingKey = pair.Key;
                var bufferPool = pair.Value;

                var snapshot = bufferPool.GetSnapshot();
                if (snapshot == null)
                    continue;

                foreach (var buffer in snapshot)
                {
                    yield return (routingKey, buffer);
                }
            }
        }

        private IEnumerable<IDataBatch> HandleBuffer(string routingKey, IBuffer buffer, DataBatchBuildingContext context)
        {
            while (true)
            {
                if (context.CurrentMessageBuilder.TryAppend(routingKey, buffer))
                {
                    context.CurrentBuffers.Add(buffer);
                    yield break;
                }

                if (context.IsEmpty)
                {
                    LogDroppingLargeBuffer(buffer);
                    buffer.DiscardSnapshot();
                    yield break;
                }

                yield return context.CreateBatch();

                context.Reset();
            }
        }

        private void LogDroppingLargeBuffer(IBuffer buffer)
        {
            log.Warn($"Dropping contents of internal buffer of size {buffer.SnapshotLength.Bytes()} with {buffer.SnapshotCount} records. It does not fit into batch buffer size {messageBuffer.Length.Bytes()}.");
        }
    }
}
