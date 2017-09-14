using System;

namespace Vostok.Flow.DistributedContextSerializer
{
    public abstract class BaseTypedSerializer<T> : ITypedSerializer
    {
        public Type Type => typeof(T);

        public bool TrySerialize(object value, out string serializedValue)
            => TrySerialize((T) value, out serializedValue);

        public bool TryDeserialize(string serializedValue, out object value)
        {
            var result = TryDeserialize(serializedValue, out T concreteValue);
            value = concreteValue;
            return result;
        }

        protected virtual bool TrySerialize(T value, out string serializedValue)
        {
            serializedValue = value.ToString();
            return true;
        }

        protected abstract bool TryDeserialize(string serializedValue, out T value);
    }
}