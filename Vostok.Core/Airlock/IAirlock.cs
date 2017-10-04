using System;

namespace Vostok.Airlock
{
    public interface IAirlock
    {
        void Push<T>(string routingKey, T item, DateTimeOffset timestamp = default(DateTimeOffset));
    }
}