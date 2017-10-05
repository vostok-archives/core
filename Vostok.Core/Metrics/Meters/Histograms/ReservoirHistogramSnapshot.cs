using System;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Metrics.Meters.Histograms
{
    public class ReservoirHistogramSnapshot
    {
        public ReservoirHistogramSnapshot(
            IReadOnlyList<double> sortedValues,
            long measurementsCount)
        {
            MeasurementsCount = measurementsCount;
            Sample = sortedValues;
            Min = sortedValues[0];
            Max = sortedValues[sortedValues.Count - 1];
            Mean = sortedValues.Average();
            StdDev = CalculateStdDev(sortedValues);
        }

        /// <summary>
        /// Sorted sample which approximate raw stream of values
        /// </summary>
        public IReadOnlyList<double> Sample { get; }
        public long MeasurementsCount { get; }
        public double Min { get; }
        public double Max { get; }

        public double Median => GetUpperQuantile(0.5);

        public double Mean { get; }
        public double StdDev { get; }

        /// <param name="quantile">From 0 to 1</param>
        public double GetUpperQuantile(double quantile)
        {
            if (quantile < 0.0 || quantile > 1.0 || double.IsNaN(quantile))
            {
                throw new ArgumentException($"Quantile '{quantile}' should be in [0..1]");
            }

            if (Sample.Count == 0)
            {
                return 0;
            }

            var position = quantile*(Sample.Count + 1);
            var index = (int) position;

            if (index < 1)
            {
                return Sample[0];
            }

            if (index >= Sample.Count)
            {
                return Sample[Sample.Count - 1];
            }

            var lower = Sample[index - 1];
            var upper = Sample[index];

            return lower + (position - Math.Floor(position)) * (upper - lower);
        }

        private static double CalculateStdDev(IReadOnlyList<double> values)
        {
            if (values.Count <= 1)
            {
                return 0;
            }

            var avg = values.Average();
            var sum = values.Sum(d => (d - avg)*(d - avg));
            return Math.Sqrt(sum / (values.Count - 1));
        }
    }
}