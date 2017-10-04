using System;
using System.Threading;

namespace Vostok.Airlock
{
    public static class AirlockSerializerRegistry
    {
        public static void Register<T>(IAirlockSerializer<T> serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            Interlocked.Exchange(ref Container<T>.Serializer, serializer);
        }

        public static bool TryGet<T>(out IAirlockSerializer<T> serializer)
        {
            return (serializer = Container<T>.Serializer) != null;
        }

        private static class Container<T>
        {
            public static IAirlockSerializer<T> Serializer;
        }
    }
}
