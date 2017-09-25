using System.Threading;

namespace Vostok.Metrics
{
    public class SumMeter : IMeter<long>
    {
        private long count;

        public void Add(long value)
        {
            Interlocked.Add(ref count, value);
        }

        public long Reset()
        {
            return Interlocked.Exchange(ref count, 0);
        }
    }
}