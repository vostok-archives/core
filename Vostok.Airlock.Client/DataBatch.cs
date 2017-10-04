using System;
using System.Collections.Generic;

namespace Vostok.Airlock
{
    internal class DataBatch : IDataBatch
    {
        public DataBatch(ArraySegment<byte> serializedMessage, IList<IBuffer> participatingBuffers)
        {
            SerializedMessage = serializedMessage;
            ParticipatingBuffers = participatingBuffers;
        }

        public ArraySegment<byte> SerializedMessage { get; }

        public IList<IBuffer> ParticipatingBuffers { get; }
    }
}