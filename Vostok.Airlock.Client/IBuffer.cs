using System;

namespace Vostok.Airlock
{
    internal interface IBuffer : IAirlockSink
    {
        Guid Id { get; }

        int Position { get; }
        byte[] InternalBuffer { get; }
        int SnapshotLength { get; }
        int SnapshotCount { get; }

        void MakeSnapshot();
        void RemoveSnapshot();
        void ResetSnapshot();

        void IncrementWrittenCount();
    }
}