using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vostok.Flow.DistributedContextSerializer
{
    internal class Serializer
    {
        private readonly Dictionary<Type, ITypedSerializer> typedSerializeInfosByType;
        private readonly Dictionary<string, ITypedSerializer> typedSerializeInfosById;
        private static ITypedSerializer[] typedSerializers;

        private Serializer(ITypedSerializer[] typedSerializers)
        {
            typedSerializeInfosByType = typedSerializers.ToDictionary(x => x.Type);
            typedSerializeInfosById = typedSerializers.ToDictionary(x => x.Id);
        }

        static Serializer()
        {
            typedSerializers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => !type.IsAbstract && type.IsClass)
                .Where(type => typeof(ITypedSerializer).IsAssignableFrom(type))
                .Select(Activator.CreateInstance)
                .OfType<ITypedSerializer>()
                .ToArray();
        }

        public bool TrySerialize(object value, out string stringValue)
        {
            if (!typedSerializeInfosByType.TryGetValue(value.GetType(), out var typedSerializer))
            {
                stringValue = null;
                return false;
            }

            if (!typedSerializer.TrySerialize(value, out var serializedValue))
            {
                stringValue = null;
                return false;
            }

            stringValue = $"{typedSerializer.Id}|{serializedValue}";
            return true;
        }

        public bool TryDeserialize(string stringValue, out object value)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                value = null;
                return false;
            }

            var split = stringValue.Split(new []{'|'}, 2, StringSplitOptions.None);
            if (split.Length < 2)
            {
                value = null;
                return false;
            }

            var typeId = split[0];
            if (!typedSerializeInfosById.TryGetValue(typeId, out var typedSerializer))
            {
                value = null;
                return false;
            }

            return typedSerializer.TryDeserialize(split[1], out value);
        }

        public static Serializer Create()
        {
            return new Serializer(typedSerializers);
        }
    }
}