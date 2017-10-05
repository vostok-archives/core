using System.Threading;

namespace Vostok.Commons.Synchronization
{
    public class AtomicBoolean
    {
        private const int TrueState = 1;
        private const int FalseState = 0;

        private int state;

        public AtomicBoolean(bool initialValue)
        {
            state = initialValue ? TrueState : FalseState;
        }

        public bool Value
        {
            get => Interlocked.CompareExchange(ref state, 0, 0) == TrueState;
            set => Interlocked.Exchange(ref state, value ? TrueState : FalseState);
        }

        public bool TrySetTrue()
        {
            return Interlocked.CompareExchange(ref state, TrueState, FalseState) == FalseState;
        }

        public bool TrySetFalse()
        {
            return Interlocked.CompareExchange(ref state, FalseState, TrueState) == TrueState;
        }

        public static implicit operator bool(AtomicBoolean atomicBoolean)
        {
            return atomicBoolean.Value;
        }
    }
}
