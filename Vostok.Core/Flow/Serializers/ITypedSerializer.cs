using System;

namespace Vostok.Flow.Serializers
{
    internal interface ITypedSerializer
    {
        string Id { get; }
        Type Type { get; }
        bool TrySerialize(object value, out string serializedValue);
        bool TryDeserialize(string serializedValue, out object value);
    }
}