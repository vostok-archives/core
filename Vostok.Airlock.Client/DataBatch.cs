using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Airlock
{
    internal class DataBatch : IDataBatch
    {
        public DataBatch(ArraySegment<byte> serializedMessage, ICollection<BufferSlice> participatingBuffers)
        {
            SerializedMessage = serializedMessage;
            ParticipatingSlices = participatingBuffers;
        }

        public ArraySegment<byte> SerializedMessage { get; }

        public ICollection<BufferSlice> ParticipatingSlices { get; }

        public int ItemsCount => ParticipatingSlices.Sum(buffer => buffer.Items);
    }
}