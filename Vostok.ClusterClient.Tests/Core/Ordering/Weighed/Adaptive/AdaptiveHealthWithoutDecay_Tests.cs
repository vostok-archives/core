using System;
using FluentAssertions;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Ordering.Weighed.Adaptive;
using Xunit;

namespace Vostok.Clusterclient.Core.Ordering.Weighed.Adaptive
{
    public class AdaptiveHealthWithoutDecay_Tests
    {
        private AdaptiveHealthWithoutDecay implementation;

        public AdaptiveHealthWithoutDecay_Tests()
        {
            implementation = new AdaptiveHealthWithoutDecay(2, 0.25, 0.002);
        }

        [Theory]
        [InlineData(-1.0)]
        [InlineData(0.0)]
        [InlineData(0.1)]
        [InlineData(1.0)]
        public void Ctor_should_throw_an_error_when_up_multiplier_is_incorrect(double value)
        {
            Action action = () => implementation = new AdaptiveHealthWithoutDecay(value, 0.5, 0.01);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(0)]
        [InlineData(1.0)]
        [InlineData(1.1)]
        public void Ctor_should_throw_an_error_when_down_multiplier_is_incorrect(double value)
        {
            Action action = () => implementation = new AdaptiveHealthWithoutDecay(2, value, 0.01);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(0)]
        [InlineData(1.0)]
        [InlineData(1.1)]
        public void Ctor_should_throw_an_error_when_minimum_health_value_is_incorrect(double value)
        {
            Action action = () => implementation = new AdaptiveHealthWithoutDecay(2, 0.5, value);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Fact]
        public void ModifyWeight_should_multiply_weight_by_health_value()
        {
            var weight = 0.5d;

            implementation.ModifyWeight(0.1, ref weight);

            weight.Should().Be(0.05d);
        }

        [Fact]
        public void CreateDefaultHealth_should_return_health_equal_to_one()
        {
            implementation.CreateDefaultHealth().Should().Be(1.0d);
        }

        [Fact]
        public void IncreaseHealth_should_multiply_value_by_up_multiplier()
        {
            implementation.IncreaseHealth(0.2).Should().Be(0.4);
        }

        [Fact]
        public void IncreaseHealth_should_not_exceed_maximum_health_value()
        {
            implementation.IncreaseHealth(0.6).Should().Be(1.0);
        }

        [Fact]
        public void DecreaseHealth_should_multiply_value_by_down_multiplier()
        {
            implementation.DecreaseHealth(0.8).Should().Be(0.2);
        }

        [Fact]
        public void DecreaseHealth_should_not_exceed_minimum_health_value()
        {
            implementation.DecreaseHealth(0.003).Should().Be(0.002);
        }

        [Fact]
        public void AreEqual_should_return_true_for_equal_values()
        {
            implementation.AreEqual(0.5, 0.5).Should().BeTrue();
        }

        [Fact]
        public void AreEqual_should_return_false_for_different_values()
        {
            implementation.AreEqual(0.5, 0.500001).Should().BeFalse();
        }
    }
}
