using System;

namespace Vostok.Airlock
{
    internal interface IRecordWriter
    {
        void Write<T>(T item, IAirlockSerializer<T> serializer, DateTimeOffset timestamp, IBufferPool bufferPool);
    }
}