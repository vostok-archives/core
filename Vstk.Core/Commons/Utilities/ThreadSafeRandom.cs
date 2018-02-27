using System;
using Vstk.Commons.Model;

namespace Vstk.Commons.Utilities
{
    public static class ThreadSafeRandom
    {
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

        public static long Next(long minValue, long maxValue)
        {
            return Math.Abs(BitConverter.ToInt64(NextBytes(8), 0) % (maxValue - minValue)) + minValue;
        }

        public static void NextBytes(byte[] buffer)
        {
            ObtainRandom().NextBytes(buffer);
        }

        public static byte[] NextBytes(DataSize size)
        {
            return NextBytes(size.Bytes);
        }

        public static byte[] NextBytes(long size)
        {
            var buffer = new byte[size];

            NextBytes(buffer);

            return buffer;
        }

        public static bool FlipCoin()
        {
            return NextDouble() <= 0.5;
        }

        private static Random ObtainRandom()
        {
            return random ?? (random = new Random(Guid.NewGuid().GetHashCode()));
        }

        [ThreadStatic]
        private static Random random;
    }
}