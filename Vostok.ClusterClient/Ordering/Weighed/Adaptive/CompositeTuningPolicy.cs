using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    /// <summary>
    /// <para>Represents a policy which combines results of several other policies using given priority list:</para>
    /// <list type="number">
    /// <item>If any of policies select to <see cref="AdaptiveHealthAction.Decrease"/> replica health, it gets decreased.</item>
    /// <item>If any of policies select to <see cref="AdaptiveHealthAction.Increase"/> replica health, it gets increased.</item>
    /// <item>If none of policies select to increase or decrease replica health, it isn't changed.</item>
    /// </list>
    /// </summary>
    public class CompositeTuningPolicy : IAdaptiveHealthTuningPolicy
    {
        private readonly IAdaptiveHealthTuningPolicy[] policies;

        public CompositeTuningPolicy([NotNull] params IAdaptiveHealthTuningPolicy[] policies)
        {
            if (policies == null)
                throw new ArgumentNullException(nameof(policies));

            this.policies = policies;
        }

        public AdaptiveHealthAction SelectAction(ReplicaResult result)
        {
            var seenIncrease = false;
            var seenDecrease = false;

            for (var i = 0; i < policies.Length; i++)
            {
                var action = policies[i].SelectAction(result);

                if (action == AdaptiveHealthAction.Increase)
                    seenIncrease = true;

                if (action == AdaptiveHealthAction.Decrease)
                    seenDecrease = true;
            }

            if (seenDecrease)
                return AdaptiveHealthAction.Decrease;

            if (seenIncrease)
                return AdaptiveHealthAction.Increase;

            return AdaptiveHealthAction.DontTouch;
        }
    }
}
