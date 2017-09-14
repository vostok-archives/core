using System.Collections.Generic;
using System.Linq;

namespace Vostok.Flow.DistributedContextSerializer
{
    internal class Serializer
    {
        private readonly Dictionary<string, ITypedSerializer> typedSerializeInfosByTypeName;

        private Serializer(params ITypedSerializer[] typedSerializers)
        {
            typedSerializeInfosByTypeName = typedSerializers.ToDictionary(x => x.Type.Name);
        }

        public bool TrySerialize(object value, out string stringValue)
        {
            if (!typedSerializeInfosByTypeName.TryGetValue(value.GetType().Name, out var typedSerializer))
            {
                stringValue = null;
                return false;
            }

            if (!typedSerializer.TrySerialize(value, out var serializedValue))
            {
                stringValue = null;
                return false;
            }

            stringValue = $"{typedSerializer.Type.Name}|{serializedValue}";
            return true;
        }

        public bool TryDeserialize(string stringValue, out object value)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                value = null;
                return false;
            }

            var split = stringValue.Split(new []{'|'}, 2);
            if (split.Length < 2)
            {
                value = null;
                return false;
            }

            var typeName = split[0];
            if (!typedSerializeInfosByTypeName.TryGetValue(typeName, out var typedSerializer))
            {
                value = null;
                return false;
            }

            return typedSerializer.TryDeserialize(stringValue.Substring(2), out value);
        }

        public static Serializer Create()
        {
            return new Serializer(
                );
        }
    }
}