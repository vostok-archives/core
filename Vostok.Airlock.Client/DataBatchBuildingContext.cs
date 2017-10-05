using System.Collections.Generic;
using Vostok.Commons.Utilities;

namespace Vostok.Airlock
{
    internal class DataBatchBuildingContext
    {
        private readonly byte[] messageBuffer;

        public DataBatchBuildingContext(byte[] messageBuffer)
        {
            this.messageBuffer = messageBuffer;

            Reset();
        }

        public HashSet<IBuffer> CurrentBuffers { get; private set; }

        public RequestMessageBuilder CurrentMessageBuilder { get; private set; }

        public bool IsEmpty => CurrentBuffers.Count == 0;

        public void Reset()
        {
            CurrentBuffers = new HashSet<IBuffer>(ReferenceEqualityComparer<IBuffer>.Instance);
            CurrentMessageBuilder = new RequestMessageBuilder(messageBuffer);
        }

        public DataBatch CreateBatch()
        {
            return new DataBatch(CurrentMessageBuilder.Message, CurrentBuffers);
        }
    }
}