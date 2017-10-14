namespace Vostok.Airlock
{
    internal interface IBuffer : IAirlockSink
    {
        int Position { get; set; }
        int WrittenRecords { get; set; }

        byte[] InternalBuffer { get; }

        int SnapshotLength { get; }
        int SnapshotCount { get; }

        void MakeSnapshot();
        void DiscardSnapshot();
        void CollectGarbage();
    }
}