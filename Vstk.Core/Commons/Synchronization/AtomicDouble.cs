using System.Threading;

namespace Vstk.Commons.Synchronization
{
    public class AtomicDouble : IAtomicNumber<double>
    {
        private double value;

        public AtomicDouble()
        {
        }

        public AtomicDouble(double value)
        {
            this.value = value;
        }

        public double Value
        {
            get => Interlocked.CompareExchange(ref value, 0, 0);
            set => Interlocked.Exchange(ref this.value, value);
        }

        public double Increment()
        {
            return Add(1);
        }

        public double Decrement()
        {
            return Add(-1);
        }

        public double Add(double diff)
        {
            var newCurrentValue = value;
            while (true)
            {
                var currentValue = newCurrentValue;
                var newValue = currentValue + diff;
                newCurrentValue = Interlocked.CompareExchange(ref value, newValue, currentValue);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (newCurrentValue == currentValue)
                    return newValue;
            }
        }

        public bool TrySet(double newValue, double expectedValue)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Interlocked.CompareExchange(ref value, newValue, expectedValue) == expectedValue;
        }

        public bool TryIncreaseTo(double newValue)
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

        public static implicit operator double(AtomicDouble atomicDouble)
        {
            return atomicDouble.Value;
        }
    }

}