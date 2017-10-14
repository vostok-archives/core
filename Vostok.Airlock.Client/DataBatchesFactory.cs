using System;
using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal class DataBatchesFactory : IDataBatchesFactory
    {
        private readonly IReadOnlyDictionary<string, IBufferPool> bufferPools;
        private readonly IBufferSliceFactory bufferSliceFactory;
        private readonly byte[] messageBuffer;

        public DataBatchesFactory(
            IReadOnlyDictionary<string, IBufferPool> bufferPools, 
            IBufferSliceFactory bufferSliceFactory, 
            byte[] messageBuffer)
        {
            this.bufferPools = bufferPools;
            this.bufferSliceFactory = bufferSliceFactory;
            this.messageBuffer = messageBuffer;
        }

        public IEnumerable<IDataBatch> CreateBatches()
        {
            var context = new DataBatchBuildingContext(messageBuffer);

            foreach ((var routingKey, var bufferSlice) in EnumerateAllBufferSlices())
            {
                foreach (var batch in HandleSlice(routingKey, bufferSlice, context))
                {
                    yield return batch;
                }
            }

            if (!context.IsEmpty)
                yield return context.CreateBatch();
        }

        private IEnumerable<(string, BufferSlice)> EnumerateAllBufferSlices()
        {
            foreach (var pair in bufferPools)
            {
                var routingKey = pair.Key;
                var bufferPool = pair.Value;

                var maximumSliceLength = messageBuffer.Length 
                    - RequestMessageBuilder.CommonHeaderSize
                    - RequestMessageBuilder.EstimateEventGroupHeaderSize(routingKey);

                var snapshot = bufferPool.GetSnapshot();
                if (snapshot == null)
                    continue;

                foreach (var buffer in snapshot)
                {
                    foreach (var slice in bufferSliceFactory.Cut(buffer, maximumSliceLength))
                    {
                        yield return (routingKey, slice);
                    }
                }
            }
        }

        private IEnumerable<IDataBatch> HandleSlice(string routingKey, BufferSlice slice, DataBatchBuildingContext context)
        {
            if (context.CurrentMessageBuilder.TryAppend(routingKey, slice))
            {
                context.CurrentSlices.Add(slice);
                yield break;
            }

            if (context.IsEmpty)
                throw new Exception($"Bug! Somehow there's a buffer slice of size {slice.Length} that does not fit into max batch size {messageBuffer.Length} with overhead considered.");

            yield return context.CreateBatch();

            context.Reset();
        }
    }
}
