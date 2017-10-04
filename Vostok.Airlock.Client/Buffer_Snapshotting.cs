namespace Vostok.Airlock
{
    internal partial class Buffer
    {
        public int SnapshotLength { get; private set; }
        public int SnapshotCount { get; private set; }

        public void MakeSnapshot()
        {
            SnapshotLength = Position;
            SnapshotCount = writtenCount;
        }

        public void RemoveSnapshot()
        {
            SnapshotLength = 0;
            SnapshotCount = 0;
        }

        public void ResetSnapshot()
        {
            if (SnapshotLength > 0)
            {
                if (SnapshotLength != Position)
                {
                    var bytesWrittenAfterSnapshot = Position - SnapshotLength;
                    System.Buffer.BlockCopy(InternalBuffer, SnapshotLength, InternalBuffer, 0, bytesWrittenAfterSnapshot);
                    Position = bytesWrittenAfterSnapshot;
                }
                else
                {
                    binaryWriter.Reset();
                }
                SnapshotLength = 0;
            }

            if (SnapshotCount > 0)
            {
                writtenCount -= SnapshotCount;
                SnapshotCount = 0;
            }
        }
    }
}
