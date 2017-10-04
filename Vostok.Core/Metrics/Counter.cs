using System.Threading;

namespace Vostok.Metrics
{
    public class Counter : ICounter
    {
        private long count;

        public long GetValue() => Interlocked.Read(ref count);

        public void Add(long value = 1)
        {
            Interlocked.Add(ref count, value);
        }

        public long Reset()
        {
            return Interlocked.Exchange(ref count, 0);
        }
    }
}