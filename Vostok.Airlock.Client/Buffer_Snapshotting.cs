namespace Vostok.Airlock
{
    internal partial class Buffer
    {
        public int SnapshotLength { get; private set; }
        public int SnapshotCount { get; private set; }
        public bool SnapshotIsGarbage { get; private set; }

        public void MakeSnapshot()
        {
            SnapshotLength = Position;
            SnapshotCount = WrittenRecords;
            SnapshotIsGarbage = false;
        }

        public void CancelSnapshot()
        {
            SnapshotLength = 0;
            SnapshotCount = 0;
            SnapshotIsGarbage = false;
        }

        public void ScheduleGarbageCollection()
        {
            SnapshotIsGarbage = true;
        }

        public void CollectGarbageIfScheduled()
        {
            if (!SnapshotIsGarbage)
                return;

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
                WrittenRecords -= SnapshotCount;
                SnapshotCount = 0;
            }

            SnapshotIsGarbage = false;
        }
    }
}
