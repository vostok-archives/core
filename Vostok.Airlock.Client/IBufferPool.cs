using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal interface IBufferPool
    {
        bool TryAcquire(out IBuffer buffer);

        void Release(IBuffer buffer);

        List<IBuffer> GetSnapshot();
    }
}