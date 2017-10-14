using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal interface IBufferSliceFactory
    {
        IEnumerable<BufferSlice> Cut(IBuffer buffer, int maximumSliceLength);
    }
}