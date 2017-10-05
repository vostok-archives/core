using System.Collections.Concurrent;
using System.Collections.Generic;
using Vostok.Commons.Binary;
using Vostok.Commons.Utilities;

namespace Vostok.Airlock
{
    internal class BufferPool : IBufferPool
    {
        private readonly IMemoryManager memoryManager;
        private readonly ConcurrentQueue<IBuffer> buffers;
        private readonly HashSet<IBuffer> snapshotSieve;

        public BufferPool(IMemoryManager memoryManager, int initialBuffersCount)
        {
            this.memoryManager = memoryManager;

            buffers = new ConcurrentQueue<IBuffer>();

            for (var i = 0; i < initialBuffersCount; i++)
            {
                if (TryCreateBuffer(out var buffer))
                {
                    buffers.Enqueue(buffer);
                }
                else break;
            }

            snapshotSieve = new HashSet<IBuffer>(ReferenceEqualityComparer<IBuffer>.Instance);
        }

        public bool TryAcquire(out IBuffer buffer)
        {
            var result = buffers.TryDequeue(out buffer) || TryCreateBuffer(out buffer);
            if (result)
                buffer.CollectGarbage();

            return result;
        }

        public void Release(IBuffer buffer)
        {
            buffers.Enqueue(buffer);
        }

        // Рассчитываем на то, что GetSnapshot() всегда вызывается последовательно в одной таске.
        public List<IBuffer> GetSnapshot()
        {
            snapshotSieve.Clear();

            var initialCount = buffers.Count;
            var snapshot = null as List<IBuffer>;

            // Do x2 iterations to meet certain statistical requirements. This highly important constant was deduced with extensive load testing.
            for (var i = 0; i < initialCount * 2; i++)
            {
                if (!buffers.TryDequeue(out var buffer))
                    break;

                if (!snapshotSieve.Add(buffer))
                {
                    buffers.Enqueue(buffer);
                    continue;
                }

                buffer.CollectGarbage();

                if (buffer.Position > 0)
                {
                    buffer.MakeSnapshot();
                    (snapshot ?? (snapshot = new List<IBuffer>())).Add(buffer);
                }

                buffers.Enqueue(buffer);
            }

            return snapshot;
        }

        private bool TryCreateBuffer(out IBuffer buffer)
        {
            if (!memoryManager.TryCreateBuffer(out var byteBuffer))
            {
                buffer = null;
                return false;
            }

            var binaryWriter = new BinaryBufferWriter(byteBuffer);

            buffer = new Buffer(binaryWriter, memoryManager);
            return true;
        }
    }
}
