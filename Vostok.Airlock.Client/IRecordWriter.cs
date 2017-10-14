using System;

namespace Vostok.Airlock
{
    internal interface IRecordWriter
    {
        bool TryWrite<T>(T item, IAirlockSerializer<T> serializer, DateTimeOffset timestamp, IBufferPool bufferPool);
    }
}