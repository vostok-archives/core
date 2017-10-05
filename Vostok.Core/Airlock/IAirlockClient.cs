using System;

namespace Vostok.Airlock
{
    public interface IAirlockClient
    {
        void Push<T>(string routingKey, T item, DateTimeOffset? timestamp = null);
    }
}