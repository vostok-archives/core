using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Ordering.Weighed.Adaptive;
using Vostok.ClusterClient.Tests.Helpers;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Weighed.Adaptive
{
    public class AdaptiveHealthWithLinearDecay_Tests
    {
        private DateTime currentTime;
        private ITimeProvider timeProvider;
        private AdaptiveHealthWithLinearDecay implementation;

        [SetUp]
        public void SetUp()
        {
            timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentTime().Returns(currentTime = DateTime.UtcNow);

            implementation = new AdaptiveHealthWithLinearDecay(timeProvider, 5.Minutes(), 2, 0.25, 0.002);
        }

        
        [TestCase(-1.0)]
        [TestCase(0.0)]
        [TestCase(0.1)]
        [TestCase(1.0)]
        public void Ctor_should_throw_an_error_when_up_multiplier_is_incorrect(double value)
        {
            Action action = () => implementation = new AdaptiveHealthWithLinearDecay(timeProvider, 5.Minutes(), value, 0.5, 0.01);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        
        [TestCase(-0.1)]
        [TestCase(0)]
        [TestCase(1.0)]
        [TestCase(1.1)]
        public void Ctor_should_throw_an_error_when_down_multiplier_is_incorrect(double value)
        {
            Action action = () => implementation = new AdaptiveHealthWithLinearDecay(timeProvider, 5.Minutes(), 2, value, 0.01);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        
        [TestCase(-0.1)]
        [TestCase(0)]
        [TestCase(1.0)]
        [TestCase(1.1)]
        public void Ctor_should_throw_an_error_when_minimum_health_value_is_incorrect(double value)
        {
            Action action = () => implementation = new AdaptiveHealthWithLinearDecay(timeProvider, 5.Minutes(), 2, 0.5, value);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_decay_duration_is_negative()
        {
            Action action = () => implementation = new AdaptiveHealthWithLinearDecay(timeProvider, -5.Minutes(), 2, 0.5, 0.002);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_decay_duration_is_zero()
        {
            Action action = () => implementation = new AdaptiveHealthWithLinearDecay(timeProvider, TimeSpan.Zero, 2, 0.5, 0.002);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void ModifyWeight_should_not_modify_weight_if_current_health_has_maximum_value_and_distant_decay_pivot()
        {
            var weight = 2.0;

            var health = new HealthWithDecay(1.0, DateTime.MinValue);

            implementation.ModifyWeight(health, ref weight);

            weight.Should().Be(2.0);
        }

        [Test]
        public void ModifyWeight_should_not_modify_weight_if_current_health_has_maximum_value_and_recent_decay_pivot()
        {
            var weight = 2.0;

            var health = new HealthWithDecay(1.0, currentTime);

            implementation.ModifyWeight(health, ref weight);

            weight.Should().Be(2.0);
        }

        [Test]
        public void ModifyWeight_should_apply_full_health_damage_when_decay_pivot_is_equal_to_current_moment()
        {
            var weight = 2.0;

            var health = new HealthWithDecay(0.3, currentTime);

            implementation.ModifyWeight(health, ref weight);

            weight.Should().Be(0.6);
        }

        [Test]
        public void ModifyWeight_should_not_apply_any_health_damage_when_decay_pivot_is_in_distant_past()
        {
            var weight = 2.0;

            var health = new HealthWithDecay(0.3, currentTime - 5.Minutes());

            implementation.ModifyWeight(health, ref weight);

            weight.Should().Be(2.0);
        }

        [Test]
        public void ModifyWeight_should_partially_apply_health_damage_based_on_distance_from_decay_pivot()
        {
            var weight = 2.0;

            var health = new HealthWithDecay(0.3, currentTime - 3.Minutes());

            implementation.ModifyWeight(health, ref weight);

            weight.Should().Be(1.44); // 2.0 * (0.3 + 0.7 * (3 / 5))
        }

        [Test]
        public void CreateDefaultHealth_should_return_health_with_value_equal_to_one()
        {
            implementation.CreateDefaultHealth().Value.Should().Be(1.0);
        }

        [Test]
        public void CreateDefaultHealth_should_return_health_with_decay_pivot_from_distant_past()
        {
            implementation.CreateDefaultHealth().DecayPivot.Should().Be(DateTime.MinValue);
        }

        [Test]
        public void IncreaseHealth_should_multiply_health_value_by_up_multiplier()
        {
            var originalHealth = new HealthWithDecay(0.4, DateTime.MinValue);

            var modifiedHealth = implementation.IncreaseHealth(originalHealth);

            modifiedHealth.Value.Should().Be(0.8);
        }

        [Test]
        public void IncreaseHealth_should_not_exceed_maximum_health_value()
        {
            var originalHealth = new HealthWithDecay(0.6, DateTime.MinValue);

            var modifiedHealth = implementation.IncreaseHealth(originalHealth);

            modifiedHealth.Value.Should().Be(1.0);
        }

        [Test]
        public void IncreaseHealth_should_preserve_decay_point()
        {
            var originalHealth = new HealthWithDecay(0.6, currentTime - 2.Hours());

            var modifiedHealth = implementation.IncreaseHealth(originalHealth);

            modifiedHealth.DecayPivot.Should().Be(originalHealth.DecayPivot);
        }

        [Test]
        public void DecreaseHealth_should_multiply_health_value_by_down_multiplier()
        {
            var originalHealth = new HealthWithDecay(0.8, DateTime.MinValue);

            var modifiedHealth = implementation.DecreaseHealth(originalHealth);

            modifiedHealth.Value.Should().Be(0.2);
        }

        [Test]
        public void DecreaseHealth_should_not_exceed_minimum_health_value()
        {
            var originalHealth = new HealthWithDecay(0.003, DateTime.MinValue);

            var modifiedHealth = implementation.DecreaseHealth(originalHealth);

            modifiedHealth.Value.Should().Be(0.002);
        }

        [Test]
        public void DecreaseHealth_should_update_decay_pivot_to_current_time()
        {
            var originalHealth = new HealthWithDecay(0.8, DateTime.MinValue);

            var modifiedHealth = implementation.DecreaseHealth(originalHealth);

            modifiedHealth.DecayPivot.Should().Be(currentTime);
        }

        [Test]
        public void AreEqual_should_return_true_for_healthes_with_equal_values_and_decay_pivots()
        {
            var health1 = new HealthWithDecay(0.5, currentTime - 1.Minutes());
            var health2 = new HealthWithDecay(health1.Value, health1.DecayPivot);

            implementation.AreEqual(health1, health2).Should().BeTrue();
        }

        [Test]
        public void AreEqual_should_return_false_for_healthes_with_different_values()
        {
            var health1 = new HealthWithDecay(0.5, currentTime - 1.Minutes());
            var health2 = new HealthWithDecay(health1.Value + 0.0001, health1.DecayPivot);

            implementation.AreEqual(health1, health2).Should().BeFalse();
        }

        [Test]
        public void AreEqual_should_return_false_for_healthes_with_different_decay_pivots()
        {
            var health1 = new HealthWithDecay(0.5, currentTime - 1.Minutes());
            var health2 = new HealthWithDecay(health1.Value, health1.DecayPivot - 1.Ticks());

            implementation.AreEqual(health1, health2).Should().BeFalse();
        }
    }
}
