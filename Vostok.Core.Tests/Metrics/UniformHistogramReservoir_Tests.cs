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
            var random = new Random(12345);
            for (var i = 0; i < 100*1000; i++)
            {
                reservoir.Add(random.Next(100));
            }

            var snapshot = reservoir.Reset();

            snapshot.GetQuantile(0).Should().Be(0);
            snapshot.GetQuantile(0.25).Should().Be(25);
            snapshot.GetQuantile(0.50).Should().Be(50);
            snapshot.GetQuantile(0.75).Should().Be(75);
            snapshot.GetQuantile(0.90).Should().Be(90);
            snapshot.GetQuantile(0.95).Should().Be(95);
            snapshot.GetQuantile(0.99).Should().Be(99);
            snapshot.GetQuantile(1).Should().Be(1);
        }
    }
}