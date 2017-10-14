using System.Collections.Generic;

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

        public List<BufferSlice> CurrentSlices { get; private set; }

        public RequestMessageBuilder CurrentMessageBuilder { get; private set; }

        public bool IsEmpty => CurrentSlices.Count == 0;

        public void Reset()
        {
            CurrentSlices = new List<BufferSlice>();
            CurrentMessageBuilder = new RequestMessageBuilder(messageBuffer);
        }

        public DataBatch CreateBatch()
        {
            return new DataBatch(CurrentMessageBuilder.Message, CurrentSlices);
        }
    }
}