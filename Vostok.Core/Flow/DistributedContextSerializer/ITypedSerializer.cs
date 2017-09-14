using System;

namespace Vostok.Flow.DistributedContextSerializer
{
    internal interface ITypedSerializer
    {
        Type Type { get; }
        bool TrySerialize(object value, out string serializedValue);
        bool TryDeserialize(string serializedValue, out object value);
    }
}