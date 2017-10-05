using System;
using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal interface IDataBatch
    {
        ArraySegment<byte> SerializedMessage { get; }

        ICollection<BufferSlice> ParticipatingSlices { get; }

        int ItemsCount { get; }
    }
}