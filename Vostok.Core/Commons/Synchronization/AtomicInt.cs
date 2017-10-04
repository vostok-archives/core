using System.Threading;

namespace Vostok.Commons.Synchronization
{
    public class AtomicInt
    {
        private int value;

        public AtomicInt(int value)
        {
            this.value = value;
        }

        public int Value
        {
            get => Interlocked.CompareExchange(ref value, 0, 0);
            set => Interlocked.Exchange(ref this.value, value);
        }

        public int Increment()
        {
            return Interlocked.Increment(ref value);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref value);
        }

        public bool TrySet(int newValue, int expectedValue)
        {
            return Interlocked.CompareExchange(ref value, newValue, expectedValue) == expectedValue;
        }

        public bool TryIncreaseTo(int newValue)
        {
            while (true)
            {
                var currentValue = Value;
                if (newValue <= currentValue)
                    return false;

                if (TrySet(newValue, currentValue))
                    return true;
            }
        }

        public static implicit operator int(AtomicInt atomicInt)
        {
            return atomicInt.Value;
        }
    }
}
