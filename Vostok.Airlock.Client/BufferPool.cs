using System.Collections.Concurrent;
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
            return buffers.TryDequeue(out buffer) || TryCreateBuffer(out buffer);
        }

        public void Release(IBuffer buffer)
        {
            buffers.Enqueue(buffer);
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
