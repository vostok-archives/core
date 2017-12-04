using System;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Ordering.Weighed.Adaptive;
using Vostok.Clusterclient.Ordering.Weighed.Gray;
using Vostok.Clusterclient.Ordering.Weighed.Leadership;

namespace Vostok.Clusterclient.Ordering.Weighed
{
    public static class WeighedReplicaOrderingBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="GrayListModifier"/> with given <paramref name="grayPeriodProvider"/> to the chain.
        /// </summary>
        public static void AddGrayListModifier(this IWeighedReplicaOrderingBuilder builder, IGrayPeriodProvider grayPeriodProvider)
        {
            builder.AddModifier(new GrayListModifier(grayPeriodProvider, builder.Log));
        }

        /// <summary>
        /// Adds a <see cref="GrayListModifier"/> with given fixed <paramref name="grayPeriod"/> to the chain.
        /// </summary>
        public static void AddGrayListModifier(this IWeighedReplicaOrderingBuilder builder, TimeSpan grayPeriod)
        {
            builder.AddModifier(new GrayListModifier(new FixedGrayPeriodProvider(grayPeriod), builder.Log));
        }

        /// <summary>
        /// Adds an <see cref="AdaptiveHealthModifier{THealth}"/> with <see cref="AdaptiveHealthWithoutDecay"/> and <see cref="ResponseVerdictTuningPolicy"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="upMultiplier">A multiplier used to increase health. See <see cref="AdaptiveHealthWithoutDecay"/> for details.</param>
        /// <param name="downMultiplier">A multiplier used to decrease health. See <see cref="AdaptiveHealthWithoutDecay"/> for details.</param>
        /// <param name="minimumHealthValue">A minimum possible health value. See <see cref="AdaptiveHealthWithoutDecay"/> for details.</param>
        public static void AddAdaptiveHealthModifierWithoutDecay(
            this IWeighedReplicaOrderingBuilder builder,
            double upMultiplier = ClusterClientDefaults.AdaptiveHealthUpMultiplier,
            double downMultiplier = ClusterClientDefaults.AdaptiveHealthDownMultiplier,
            double minimumHealthValue = ClusterClientDefaults.AdaptiveHealthMinimumValue)
        {
            AddAdaptiveHealthModifierWithoutDecay(builder, TuningPolicies.ByResponseVerdict, upMultiplier, downMultiplier, minimumHealthValue);
        }

        /// <summary>
        /// Adds an <see cref="AdaptiveHealthModifier{T}"/> with <see cref="AdaptiveHealthWithoutDecay"/> and given <paramref name="tuningPolicy"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tuningPolicy">A health tuning policy. See <see cref="IAdaptiveHealthTuningPolicy"/> and <see cref="TuningPolicies"/> for details.</param>
        /// <param name="upMultiplier">A multiplier used to increase health. See <see cref="AdaptiveHealthWithoutDecay"/> for details.</param>
        /// <param name="downMultiplier">A multiplier used to decrease health. See <see cref="AdaptiveHealthWithoutDecay"/> for details.</param>
        /// <param name="minimumHealthValue">A minimum possible health value. See <see cref="AdaptiveHealthWithoutDecay"/> for details.</param>
        public static void AddAdaptiveHealthModifierWithoutDecay(
            this IWeighedReplicaOrderingBuilder builder,
            IAdaptiveHealthTuningPolicy tuningPolicy,
            double upMultiplier = ClusterClientDefaults.AdaptiveHealthUpMultiplier,
            double downMultiplier = ClusterClientDefaults.AdaptiveHealthDownMultiplier,
            double minimumHealthValue = ClusterClientDefaults.AdaptiveHealthMinimumValue)
        {
            builder.AddModifier(new AdaptiveHealthModifier<double>(new AdaptiveHealthWithoutDecay(upMultiplier, downMultiplier, minimumHealthValue), tuningPolicy, builder.Log));
        }

        /// <summary>
        /// Adds an <see cref="AdaptiveHealthModifier{T}"/> with <see cref="AdaptiveHealthWithLinearDecay"/> and <see cref="ResponseVerdictTuningPolicy"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="decayDuration">A duration of full health damage decay. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        /// <param name="upMultiplier">A multiplier used to increase health. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        /// <param name="downMultiplier">A multiplier used to decrease health. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        /// <param name="minimumHealthValue">A minimum possible health value. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        public static void AddAdaptiveHealthModifierWithLinearDecay(
            this IWeighedReplicaOrderingBuilder builder,
            TimeSpan decayDuration,
            double upMultiplier = ClusterClientDefaults.AdaptiveHealthUpMultiplier,
            double downMultiplier = ClusterClientDefaults.AdaptiveHealthDownMultiplier,
            double minimumHealthValue = ClusterClientDefaults.AdaptiveHealthMinimumValue)
        {
            AddAdaptiveHealthModifierWithLinearDecay(builder, TuningPolicies.ByResponseVerdict, decayDuration, upMultiplier, downMultiplier, minimumHealthValue);
        }

        /// <summary>
        /// Adds an <see cref="AdaptiveHealthModifier{T}"/> with <see cref="AdaptiveHealthWithLinearDecay"/> and given <paramref name="tuningPolicy"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tuningPolicy">A health tuning policy. See <see cref="IAdaptiveHealthTuningPolicy"/> and <see cref="TuningPolicies"/> for details.</param>
        /// <param name="decayDuration">A duration of full health damage decay. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        /// <param name="upMultiplier">A multiplier used to increase health. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        /// <param name="downMultiplier">A multiplier used to decrease health. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        /// <param name="minimumHealthValue">A minimum possible health value. See <see cref="AdaptiveHealthWithLinearDecay"/> for details.</param>
        public static void AddAdaptiveHealthModifierWithLinearDecay(
            this IWeighedReplicaOrderingBuilder builder,
            IAdaptiveHealthTuningPolicy tuningPolicy,
            TimeSpan decayDuration,
            double upMultiplier = ClusterClientDefaults.AdaptiveHealthUpMultiplier,
            double downMultiplier = ClusterClientDefaults.AdaptiveHealthDownMultiplier,
            double minimumHealthValue = ClusterClientDefaults.AdaptiveHealthMinimumValue)
        {
            builder.AddModifier(new AdaptiveHealthModifier<HealthWithDecay>(new AdaptiveHealthWithLinearDecay(new TimeProvider(), decayDuration, upMultiplier, downMultiplier, minimumHealthValue), tuningPolicy, builder.Log));
        }

        /// <summary>
        /// Adds a <see cref="LeadershipWeightModifier"/> with given <paramref name="leaderResultDetector"/> to the chain.
        /// </summary>
        public static void AddLeadershipModifier(this IWeighedReplicaOrderingBuilder builder, ILeaderResultDetector leaderResultDetector)
        {
            builder.AddModifier(new LeadershipWeightModifier(leaderResultDetector, builder.Log));
        }
    }
}
