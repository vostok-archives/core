using System;
using Vostok.Commons.Utilities;

namespace Vostok.Metrics.Meters.Histograms
{
    /// <summary>
    /// A random sampling reservoir of a stream of doubles. 
    /// Uses Vitter's Algorithm R to produce a statistically representative sample.
    /// See <a href="http://www.cs.umd.edu/~samir/498/vitter.pdf"> Random Sampling with a Reservoir</a>
    /// Mainly copied from java's metrics library: <a href="https://github.com/dropwizard/metrics/blob/4.0-development/metrics-core/src/main/java/com/codahale/metrics/UniformReservoir.java"></a>
    /// </summary>
    public class UniformHistogramReservoir : IReservoirHistogramMeter
    {
        /// This offfers a 99.9% confidence level with a 5% margin of error assuming a normal distribution.
        private const int reservoirSize = 1028;
        private readonly double[] values = new double[reservoirSize];

        //TODO (@ezsilmar) To use long here, we need to generate good uniformly distributed long values
        //We do not have this feature out of the box in .net, plus random has only 32-bit seed.
        //Thus implementing the feature correctly is difficult
        private int measurementsCount;

        public void Add(double value)
        {
            var count = ++measurementsCount;
            if (count <= values.Length)
            {
                values[count - 1] = value;
                return;
            }

            var rand = ThreadSafeRandom.Next(count);
            if (rand < values.Length)
            {
                values[rand] = value;
            }
        }

        public ReservoirHistogramSnapshot GetSnapshot()
        {
            var count = Math.Min(reservoirSize, measurementsCount);
            var valuesCopy = new double[count];
            Buffer.BlockCopy(values, 0, valuesCopy, 0, count*sizeof(double));
            Array.Sort(valuesCopy);
            return new ReservoirHistogramSnapshot(valuesCopy, measurementsCount);
        }

        public void Reset()
        {
            measurementsCount = 0;
        }
    }
}