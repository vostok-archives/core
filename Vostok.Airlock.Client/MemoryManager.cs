using System.Threading;

namespace Vostok.Airlock
{
    internal class MemoryManager : IMemoryManager
    {
        private readonly int initialBuffersSize;
        private readonly long maxMemoryForBuffers;
        private long currentSize;

        public MemoryManager(long maxMemoryForBuffers, int initialBuffersSize)
        {
            this.maxMemoryForBuffers = maxMemoryForBuffers;
            this.initialBuffersSize = initialBuffersSize;
        }

        public bool TryCreateBuffer(out byte[] buffer)
        {
            return (buffer = TryReserveBytes(initialBuffersSize) ? new byte[initialBuffersSize] : null) != null;
        }

        public bool TryReserveBytes(int amount)
        {
            while (true)
            {
                var tCurrentSize = currentSize;
                var newSize = tCurrentSize + amount;
                if (newSize <= maxMemoryForBuffers)
                {
                    var originalValue = Interlocked.CompareExchange(ref currentSize, newSize, tCurrentSize);
                    if (originalValue == tCurrentSize)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
}