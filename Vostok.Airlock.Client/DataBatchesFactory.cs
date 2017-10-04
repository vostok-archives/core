using System.Collections.Generic;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataBatchesFactory : IDataBatchesFactory
    {
        private readonly IReadOnlyDictionary<string, IBufferPool> bufferPools;
        private readonly byte[] commonBuffer;
        private readonly ILog log;

        public DataBatchesFactory(IReadOnlyDictionary<string, IBufferPool> bufferPools, byte[] commonBuffer, ILog log)
        {
            this.bufferPools = bufferPools;
            this.commonBuffer = commonBuffer;
            this.log = log;
        }

        public IEnumerable<IDataBatch> CreateBatches()
        {
            var buffers = new List<IBuffer>();
            var builder = new RequestMessageBuilder(commonBuffer);

            foreach (var pair in bufferPools)
            {
                var routingKey = pair.Key;
                var bufferPool = pair.Value;

                foreach (var buffer in bufferPool.GetSnapshot())
                {
                    if (builder.TryAdd(routingKey, buffer))
                    {
                        buffers.Add(buffer);
                        continue;
                    }

                    if (buffers.Count == 0)
                    {
                        LogDroppingLargeBuffer(buffer);
                        buffer.ScheduleGarbageCollection();
                        buffer.CollectGarbageIfScheduled();
                        continue;
                    }

                    yield return new DataBatch(builder.Message, buffers);

                    builder = new RequestMessageBuilder(commonBuffer);
                    buffers = new List<IBuffer>();
                }
            }

            if (buffers.Count > 0)
                yield return new DataBatch(builder.Message, buffers);
        }

        private void LogDroppingLargeBuffer(IBuffer buffer)
        {
            log.Warn($"Dropping contents of internal buffer of size {buffer.SnapshotLength.Bytes()}. It does not fit into batch buffer size {commonBuffer.Length.Bytes()}.");
        }
    }
}