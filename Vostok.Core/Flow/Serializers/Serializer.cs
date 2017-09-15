using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Flow.Serializers
{
    internal class Serializer
    {
        private const char separtor = '|';
        private static readonly char[] separators = new []{separtor};
        private static readonly Dictionary<string, ITypedSerializer> serializersById;
        private static readonly Dictionary<Type, ITypedSerializer> serializersByType;

        static Serializer()
        {
            var serializers = typeof(ITypedSerializer).Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && type.IsClass)
                .Where(type => typeof(ITypedSerializer).IsAssignableFrom(type))
                .Select(Activator.CreateInstance)
                .OfType<ITypedSerializer>()
                .ToArray();

            serializersById = serializers.ToDictionary(x => x.Id.ToLower());
            serializersByType = serializers.ToDictionary(x => x.Type);
        }

        public bool TrySerialize(object value, out string stringValue)
        {
            if (value == null)
            {
                stringValue = null;
                return false;
            }

            if (!serializersByType.TryGetValue(value.GetType(), out var typedSerializer))
            {
                stringValue = null;
                return false;
            }

            if (!typedSerializer.TrySerialize(value, out var serializedValue))
            {
                stringValue = null;
                return false;
            }

            stringValue = typedSerializer.Id + separtor + serializedValue;
            return true;
        }

        public bool TryDeserialize(string stringValue, out object value)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                value = null;
                return false;
            }

            var split = stringValue.Split(separators, 2, StringSplitOptions.None);
            if (split.Length < 2)
            {
                value = null;
                return false;
            }

            var typeId = split[0].ToLower();
            if (!serializersById.TryGetValue(typeId, out var typedSerializer))
            {
                value = null;
                return false;
            }

            return typedSerializer.TryDeserialize(split[1], out value);
        }
    }
}