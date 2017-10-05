using System;
using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal interface IDataBatch
    {
        ArraySegment<byte> SerializedMessage { get; }

        ICollection<IBuffer> ParticipatingBuffers { get; }

        int ItemsCount { get; }
    }
}