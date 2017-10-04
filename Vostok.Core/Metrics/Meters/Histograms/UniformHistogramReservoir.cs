using System;
using System.Threading;
using Vostok.Commons.Utilities;

namespace Vostok.Metrics.Meters.Histograms
{
    public class UniformHistogramReservoir : IHistogramReservoir
    {
        private const int ReservoirSize = 1028*27;
        private readonly double[] values = new double[ReservoirSize];

        private long measurementsCount;

        public void Add(double value)
        {
            var count = Interlocked.Increment(ref measurementsCount);
            if (count <= values.Length)
            {
                values[count - 1] = value;
                return;
            }

            var rand = ThreadSafeRandom.NextLong(count);
            if (rand < values.Length)
            {
                values[(int) rand] = value;
            }
        }

        public HistogramSnapshot GetSnapshot()
        {
            var valuesCopy = new double[ReservoirSize];
            Buffer.BlockCopy(values, 0, valuesCopy, 0, ReservoirSize*sizeof(double));
            Array.Sort(valuesCopy);
            return new HistogramSnapshot(valuesCopy, Interlocked.Read(ref measurementsCount));
        }

        public void Reset()
        {
            Interlocked.Exchange(ref measurementsCount, 0);
        }
    }
}