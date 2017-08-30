using System;

namespace Vostok.Clusterclient.Helpers
{
    internal static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random random;

        public static double NextDouble()
        {
            return ObtainRandom().NextDouble();
        }

        public static int Next()
        {
            return ObtainRandom().Next();
        }

        public static int Next(int maxValue)
        {
            return ObtainRandom().Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return ObtainRandom().Next(minValue, maxValue);
        }

        private static Random ObtainRandom()
        {
            return random ?? (random = new Random(Guid.NewGuid().GetHashCode()));
        }
    }
}
