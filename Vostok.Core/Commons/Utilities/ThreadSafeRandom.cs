using System;
using Vostok.Commons.Model;

namespace Vostok.Commons.Utilities
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

        public static long NextLong()
        {
            long result = 0;
            for (var i = 0; i < 8; i++)
            {
                var b = ObtainRandom().Next()%256;
                result += b;
                result <<= 8;
            }
            return Math.Abs(result);
        }

        public static long NextLong(long maxValue)
        {
            long bits, val;
            do
            {
                bits = NextLong() & (~(1L << 63));
                val = bits % maxValue;
            } while (bits - val + (maxValue - 1) < 0L);
            return val;
        }

        public static long NextLong(long minValue, long maxValue)
        {
            return NextLong(maxValue - minValue) + minValue;
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