using System;

namespace Vostok.Flow.Serializers
{
    internal abstract class BaseTypedSerializer<T> : ITypedSerializer
    {
        public abstract string Id { get; }
        public Type Type => typeof(T);

        public bool TrySerialize(object value, out string serializedValue)
            => TrySerialize((T) value, out serializedValue);

        public bool TryDeserialize(string serializedValue, out object value)
        {
            var result = TryDeserialize(serializedValue, out T concreteValue);
            value = concreteValue;
            return result;
        }

        protected abstract bool TrySerialize(T value, out string serializedValue);

        protected abstract bool TryDeserialize(string serializedValue, out T value);
    }
}