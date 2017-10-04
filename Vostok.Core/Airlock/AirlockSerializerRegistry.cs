using System;
using System.Threading;

namespace Vostok.Airlock
{
    public static class AirlockSerializerRegistry
    {
        public static void Register<T>(string dataType, IAirlockSerializer<T> serializer)
        {
            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            Interlocked.Exchange(ref Container<T>.DataType, dataType);
            Interlocked.Exchange(ref Container<T>.Serializer, serializer);
        }

        public static bool TryGet<T>(out string dataType, out IAirlockSerializer<T> serializer)
        {
            dataType = Container<T>.DataType;
            serializer = Container<T>.Serializer;

            return dataType != null && serializer != null;
        }

        private static class Container<T>
        {
            public static string DataType;
            public static IAirlockSerializer<T> Serializer;
        }
    }
}
