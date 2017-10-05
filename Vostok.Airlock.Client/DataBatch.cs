using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Airlock
{
    internal class DataBatch : IDataBatch
    {
        public DataBatch(ArraySegment<byte> serializedMessage, ICollection<IBuffer> participatingBuffers)
        {
            SerializedMessage = serializedMessage;
            ParticipatingBuffers = participatingBuffers;
        }

        public ArraySegment<byte> SerializedMessage { get; }

        public ICollection<IBuffer> ParticipatingBuffers { get; }

        public int ItemsCount => ParticipatingBuffers.Sum(buffer => buffer.SnapshotCount);
    }
}