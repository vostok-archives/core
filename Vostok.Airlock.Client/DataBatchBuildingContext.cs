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

        public List<IBuffer> CurrentBuffers { get; private set; }

        public RequestMessageBuilder CurrentMessageBuilder { get; private set; }

        public bool IsEmpty => CurrentBuffers.Count == 0;

        public void Reset()
        {
            CurrentBuffers = new List<IBuffer>();
            CurrentMessageBuilder = new RequestMessageBuilder(messageBuffer);
        }

        public DataBatch CreateBatch()
        {
            return new DataBatch(CurrentMessageBuilder.Message, CurrentBuffers);
        }
    }
}