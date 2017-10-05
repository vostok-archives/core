using System;
using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal interface IDataBatch
    {
        ArraySegment<byte> SerializedMessage { get; }

        IList<IBuffer> ParticipatingBuffers { get; }

        int ItemsCount { get; }
    }
}