using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Metrics.Meters.Histograms;

namespace Vostok.Metrics
{
    public class UniformHistogramReservoir_Tests
    {
        private UniformHistogramReservoir reservoir;

        [SetUp]
        public void SetUp()
        {
            reservoir = new UniformHistogramReservoir();
        }

        [Test]
        public void Random_numbers_are_spread_uniformly()
        {
            var random = new Random(12343);
            var measurementsCount = 50*1000;
            var range = 1000;

            for (var i = 0; i < measurementsCount; i++)
            {
                reservoir.Add(random.Next(range));
            }

            var snapshot = reservoir.GetSnapshot();

            var maxError = range*0.05;
            snapshot.MeasurementsCount.Should().Be(measurementsCount);
            AssertQuantile(snapshot, 0, range, maxError);
            AssertQuantile(snapshot, 0.25, range, maxError);
            AssertQuantile(snapshot, 0.50, range, maxError);
            AssertQuantile(snapshot, 0.75, range, maxError);
            AssertQuantile(snapshot, 0.90, range, maxError);
            AssertQuantile(snapshot, 0.95, range, maxError);
            AssertQuantile(snapshot, 0.99, range, maxError);
            AssertQuantile(snapshot, 0.999, range, maxError);
            AssertQuantile(snapshot, 1, range, maxError);
        }

        private static void AssertQuantile(HistogramSnapshot snapshot, double quantile, int range, double maxError)
        {
            var result = snapshot.GetQuantile(quantile);
            Console.WriteLine($"{quantile:F3}: {result:F3}");
            result.Should().BeApproximately(range*quantile, maxError);
        }
    }
}