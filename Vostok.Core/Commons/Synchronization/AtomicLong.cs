using System.Threading;

namespace Vostok.Commons.Synchronization
{
    public class AtomicLong 
    {
        private long value;

        public AtomicLong(long value)
        {
            this.value = value;
        }

        public long Value
        {
            get => Interlocked.Read(ref value);
            set => Interlocked.Exchange(ref this.value, value);
        }

        public long Increment()
        {
            return Interlocked.Increment(ref value);
        }

        public long Decrement()
        {
            return Interlocked.Decrement(ref value);
        }

        public long Add(long diff)
        {
            return Interlocked.Add(ref value, diff);
        }

        public bool TrySet(long newValue, long expectedValue)
        {
            return Interlocked.CompareExchange(ref value, newValue, expectedValue) == expectedValue;
        }

        public bool TryIncreaseTo(long newValue)
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

        public static implicit operator long(AtomicLong atomicLong)
        {
            return atomicLong.Value;
        }
    }
}
