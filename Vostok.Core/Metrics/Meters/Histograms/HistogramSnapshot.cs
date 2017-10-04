using System;
using System.Collections.Generic;

namespace Vostok.Metrics.Meters.Histograms
{
    public class HistogramSnapshot
    {
        public HistogramSnapshot(IReadOnlyList<double> values)
        {
            Values = values;
        }

        public IReadOnlyList<double> Values { get; }

        /// <param name="quantile">From 0 to 1</param>
        public double GetQuantile(double quantile)
        {
            throw new NotImplementedException();
        }
    }
}