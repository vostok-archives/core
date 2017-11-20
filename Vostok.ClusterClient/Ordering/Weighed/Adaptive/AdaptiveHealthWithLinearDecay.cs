using System;
using Vostok.Clusterclient.Helpers;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    /// <summary>
    /// <para>An implementation of adaptive health which uses numbers in <c>(0; 1]</c> range as health values. Default health value is equal to 1.</para>
    /// <para>Upon increase, health value is multiplied by an up multilplier in <c>(1; +infinity)</c> range.</para>
    /// <para>Upon decrease, health value is multiplied by a down multilplier in <c>(0; 1)</c> range.</para>
    /// <para>Health values have a customizable lower bound in <c>(0; 1)</c> range.</para>
    /// <para>Health damage also decays linearly during a configurable time period since last health decrease. Subsequent decreases reset decay duration.</para>
    /// <para>For instance, let's assume that we've reduced a replica's health to 0.5 just a moment ago and decay duration is 10 minutes. Then, assuming there are no other changes, health will have following values in the future:</para>
    /// <list type="bullet">
    /// <item>0.55 after 1 minute</item>
    /// <item>0.625 after 2.5 minutes</item>
    /// <item>0.75 after 5 minutes</item>
    /// <item>0.9 after 8 minutes</item>
    /// <item>1.0 after 10 minutes</item>
    /// <item>1.0 after 11 minutes</item>
    /// </list>
    /// <para>This decay mechanism helps to avoid situations where replicas which had temporary problems are still avoided when the problems resolve.</para>
    /// <para>Health application is just a multiplication of health value and current weight (health = 0.5 causes weight = 2 to turn into 1).</para>
    /// <para>This health implementation can only decrease replica weights as it's aim is to avoid misbehaving replicas.</para>
    /// </summary>
    public class AdaptiveHealthWithLinearDecay : IAdaptiveHealthImplementation<HealthWithDecay>
    {
        private const double maximumHealthValue = 1.0;

        private readonly ITimeProvider timeProvider;

        /// <param name="decayDuration">A duration during which health damage fully decays.</param>
        /// <param name="upMultiplier">A multiplier used to increase health. Must be in <c>(1; +infinity)</c> range.</param>
        /// <param name="downMultiplier">A multiplier used to decrease health. Must be in <c>(0; 1)</c> range.</param>
        /// <param name="minimumHealthValue">Minimum possible health value. Must be in <c>(0; 1)</c> range.</param>
        public AdaptiveHealthWithLinearDecay(
            TimeSpan decayDuration,
            double upMultiplier = ClusterClientDefaults.AdaptiveHealthUpMultiplier,
            double downMultiplier = ClusterClientDefaults.AdaptiveHealthDownMultiplier,
            double minimumHealthValue = ClusterClientDefaults.AdaptiveHealthMinimumValue)
            : this(new TimeProvider(), decayDuration, upMultiplier, downMultiplier, minimumHealthValue)
        {
        }

        internal AdaptiveHealthWithLinearDecay(
            ITimeProvider timeProvider,
            TimeSpan decayDuration,
            double upMultiplier = ClusterClientDefaults.AdaptiveHealthUpMultiplier,
            double downMultiplier = ClusterClientDefaults.AdaptiveHealthDownMultiplier,
            double minimumHealthValue = ClusterClientDefaults.AdaptiveHealthMinimumValue)
        {
            if (decayDuration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(decayDuration), $"Decay duration must be positive. Given value = '{decayDuration}'.");

            if (upMultiplier <= 1.0)
                throw new ArgumentOutOfRangeException(nameof(upMultiplier), $"Up multiplier must be > 1. Given value = '{upMultiplier}'.");

            if (downMultiplier <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(downMultiplier), $"Down multiplier must be positive. Given value = '{downMultiplier}'.");

            if (downMultiplier >= 1.0)
                throw new ArgumentOutOfRangeException(nameof(downMultiplier), $"Down multiplier must be < 1. Given value = '{downMultiplier}'.");

            if (minimumHealthValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(minimumHealthValue), $"Minimum health must be positive. Given value = '{minimumHealthValue}'.");

            if (minimumHealthValue >= 1)
                throw new ArgumentOutOfRangeException(nameof(minimumHealthValue), $"Minimum health must be < 1. Given value = '{minimumHealthValue}'.");

            this.timeProvider = timeProvider;

            DecayDuration = decayDuration;
            UpMultiplier = upMultiplier;
            DownMultiplier = downMultiplier;
            MinimumHealthValue = minimumHealthValue;
        }

        public TimeSpan DecayDuration { get; }

        public double UpMultiplier { get; }

        public double DownMultiplier { get; }

        public double MinimumHealthValue { get; }

        public void ModifyWeight(HealthWithDecay health, ref double weight)
        {
            var healthDamage = maximumHealthValue - health.Value;
            if (healthDamage <= 0.0)
                return;

            var timeSinceDecayPivot = TimeSpanExtensions.Max(timeProvider.GetCurrentTime() - health.DecayPivot, TimeSpan.Zero);
            if (timeSinceDecayPivot >= DecayDuration)
                return;

            var effectiveHealth = health.Value + healthDamage*((double) timeSinceDecayPivot.Ticks/DecayDuration.Ticks);

            weight *= effectiveHealth;
        }

        public HealthWithDecay CreateDefaultHealth()
        {
            return new HealthWithDecay(maximumHealthValue, DateTime.MinValue);
        }

        public HealthWithDecay IncreaseHealth(HealthWithDecay current)
        {
            return new HealthWithDecay(Math.Min(maximumHealthValue, current.Value*UpMultiplier), current.DecayPivot);
        }

        public HealthWithDecay DecreaseHealth(HealthWithDecay current)
        {
            return new HealthWithDecay(Math.Max(MinimumHealthValue, current.Value*DownMultiplier), timeProvider.GetCurrentTime());
        }

        public bool AreEqual(HealthWithDecay x, HealthWithDecay y)
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return x.Value.Equals(y.Value) && x.DecayPivot == y.DecayPivot;
        }

        public void LogHealthChange(Uri replica, HealthWithDecay oldHealth, HealthWithDecay newHealth, ILog log)
        {
            log.Debug($"Local base health for replica '{replica}' has changed from {oldHealth.Value:N4} to {newHealth.Value:N4}.");
        }
    }
}
