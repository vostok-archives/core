using System;

namespace Vostok.Airlock
{
    internal interface IRecordSerializer
    {
        bool TrySerialize<T>(T item, IAirlockSerializer<T> serializer, DateTimeOffset timestamp, IBuffer buffer);
    }
}