using System.Collections.Concurrent;
using System.Collections.Generic;
using Vostok.Commons.Binary;

namespace Vostok.Airlock
{
    internal class BufferPool : IBufferPool
    {
        private readonly IMemoryManager memoryManager;
        private readonly ConcurrentQueue<IBuffer> buffers;

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
        }

        public bool TryAcquire(out IBuffer buffer)
        {
            var result = buffers.TryDequeue(out buffer) || TryCreateBuffer(out buffer);
            if (result)
                buffer.CleanupSnapshot();

            return result;
        }

        public void Release(IBuffer buffer)
        {
            buffer.CleanupSnapshot();
            buffers.Enqueue(buffer);
        }

        public List<IBuffer> GetSnapshot()
        {
            List<IBuffer> snapshot = null;

            buffers.ForEachSafe(buffer =>
            {
                if (buffer.Position > 0)
                {
                    buffer.CleanupSnapshot();
                    buffer.Snapshot();
                    (snapshot ?? (snapshot = new List<IBuffer>())).Add(buffer);
                }
            });

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
