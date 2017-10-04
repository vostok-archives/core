using System;

namespace Vostok.Airlock
{
    internal interface IBuffer : IAirlockSink
    {
        Guid Id { get; }

        int Position { get; set; }
        int WrittenRecords { get; set; }

        byte[] InternalBuffer { get; }

        int SnapshotLength { get; }
        int SnapshotCount { get; }

        void MakeSnapshot();
        void CancelSnapshot();
        void CleanupSnapshot();
    }
}