using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Flow.Serializers
{
    internal class Serializer
    {
        // CR(iloktionov): Почему часть static, а часть - нет?
        private readonly Dictionary<Type, ITypedSerializer> serializersByType;
        // CR(iloktionov): case-insensitive?
        private readonly Dictionary<string, ITypedSerializer> serializersById;
        private static readonly ITypedSerializer[] serializers;

        private Serializer(ITypedSerializer[] typedSerializers)
        {
            serializersByType = typedSerializers.ToDictionary(x => x.Type);
            serializersById = typedSerializers.ToDictionary(x => x.Id);
        }

        static Serializer()
        {
            serializers = typeof(ITypedSerializer).Assembly
                .GetTypes()
                .Where(type => !type.IsAbstract && type.IsClass)
                .Where(type => typeof(ITypedSerializer).IsAssignableFrom(type))
                .Select(Activator.CreateInstance)
                .OfType<ITypedSerializer>()
                .ToArray();
        }

        public bool TrySerialize(object value, out string stringValue)
        {
            // CR(iloktionov): А что, если value == null?
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

            stringValue = typedSerializer.Id + "|" + serializedValue;
            return true;
        }

        public bool TryDeserialize(string stringValue, out object value)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                value = null;
                return false;
            }

            // CR(iloktionov): 1. Не обязательно каждый раз создавать массив разделителей.
            // CR(iloktionov): 2. Давай протестим случай с пустой value для строк.
            var split = stringValue.Split(new []{'|'}, 2, StringSplitOptions.None);
            if (split.Length < 2)
            {
                value = null;
                return false;
            }

            var typeId = split[0];
            if (!serializersById.TryGetValue(typeId, out var typedSerializer))
            {
                value = null;
                return false;
            }

            return typedSerializer.TryDeserialize(split[1], out value);
        }

        public static Serializer Create()
        {
            return new Serializer(serializers);
        }
    }
}