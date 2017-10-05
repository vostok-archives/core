using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Metrics.Meters.Histograms;

namespace Vostok.Metrics
{
    public class UniformHistogramReservoir_Tests
    {
        [Test, Ignore("Explicit does not work")]
        public void Smoke_test()
        {
            long success = 0;
            long failure = 0;

            var retries = 5;
            for (var i = 0; i < 50; i++)
            {
                for (var j = 10000; j <= 100*1000; j += 10000)
                {
                    for (var k = 50; k < 1000; k += 50)
                    {
                        for (var r = 0; r < retries; r++)
                        {
                            try
                            {
                                Random_numbers_are_spread_uniformly(i, j, k, false);
                                success++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                failure++;
                            }
                        }
                    }
                }
            }

            var total = success + failure;
            Console.WriteLine($"Success rate: {100*success/(double)total:F3}%. Tests count: {total}. Success: {success}. Failures: {failure}");
        }

        [TestCase(12345, 50000, 100, true)]
        public void Random_numbers_are_spread_uniformly(int seed, int measurementsCount, int range, bool log)
        {
            var reservoir = new UniformHistogramReservoir();
            var random = new Random(seed);

            for (var i = 0; i < measurementsCount; i++)
            {
                reservoir.Add(random.Next(range));
            }

            var snapshot = reservoir.GetSnapshot();

            var maxError = range*0.05;
            snapshot.MeasurementsCount.Should().Be(measurementsCount);
            AssertQuantile(snapshot, 0, range, maxError, log);
            AssertQuantile(snapshot, 0.25, range, maxError, log);
            AssertQuantile(snapshot, 0.50, range, maxError, log);
            AssertQuantile(snapshot, 0.75, range, maxError, log);
            AssertQuantile(snapshot, 0.90, range, maxError, log);
            AssertQuantile(snapshot, 0.95, range, maxError, log);
            AssertQuantile(snapshot, 0.99, range, maxError, log);
            AssertQuantile(snapshot, 0.999, range, maxError, log);
            AssertQuantile(snapshot, 1, range, maxError, log);
        }

        private static void AssertQuantile(
            HistogramSnapshot snapshot,
            double quantile,
            int range,
            double maxError,
            bool log)
        {
            var result = snapshot.GetQuantile(quantile);
            if (log)
            {
                Console.WriteLine($"{quantile:F3}: {result:F3}");
            }
            result.Should().BeApproximately(range*quantile, maxError);
        }
    }
}