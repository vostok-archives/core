using System;
using System.Collections.Generic;

namespace Vostok.Metrics.Meters.Histograms
{
    public class HistogramSnapshot
    {
        public HistogramSnapshot(IReadOnlyList<double> sortedValues, long measurementsCount)
        {
            MeasurementsCount = measurementsCount;
            SortedValues = sortedValues;
        }

        public IReadOnlyList<double> SortedValues { get; }
        public long MeasurementsCount { get; }

        /// <param name="quantile">From 0 to 1</param>
        public double GetQuantile(double quantile)
        {
            if (quantile < 0.0 || quantile > 1.0 || double.IsNaN(quantile))
            {
                throw new ArgumentException($"Quantile '{quantile}' should be in [0..1]");
            }

            if (SortedValues.Count == 0)
            {
                return 0;
            }

            var position = quantile*(SortedValues.Count + 1);
            var index = (int) position;

            if (index < 1)
            {
                return SortedValues[0];
            }

            if (index >= SortedValues.Count)
            {
                return SortedValues[SortedValues.Count - 1];
            }

            var lower = SortedValues[index - 1];
            var upper = SortedValues[index];

            return lower + (position - Math.Floor(position)) * (upper - lower);
        }
    }
}