using System.Threading;

namespace Vostok.Commons.Synchronization
{
    public class AtomicBoolean
    {
        private const int trueState = 1;
        private const int falseState = 0;

        private int state;

        public AtomicBoolean(bool initialValue)
        {
            state = initialValue ? trueState : falseState;
        }

        public bool Value
        {
            get => Interlocked.CompareExchange(ref state, 0, 0) == trueState;
            set => Interlocked.Exchange(ref state, value ? trueState : falseState);
        }

        public bool TrySetTrue()
        {
            return Interlocked.CompareExchange(ref state, trueState, falseState) == falseState;
        }

        public bool TrySetFalse()
        {
            return Interlocked.CompareExchange(ref state, falseState, trueState) == trueState;
        }

        public static implicit operator bool(AtomicBoolean atomicBoolean)
        {
            return atomicBoolean.Value;
        }
    }
}
