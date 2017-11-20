using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Utilities;

namespace Vostok.Clusterclient.Strategies.TimeoutProviders
{
    /// <summary>
    /// Represents a timeout provider which issues timeouts from a fixed set of values.
    /// </summary>
    public class FixedTimeoutsProvider : ISequentialTimeoutsProvider
    {
        private readonly TimeSpan[] timeouts;
        private readonly TailTimeoutBehaviour tailBehaviour;

        public FixedTimeoutsProvider(TailTimeoutBehaviour tailBehaviour, [NotNull] params TimeSpan[] timeouts)
        {
            if (timeouts == null)
                throw new ArgumentNullException(nameof(timeouts));

            if (timeouts.Length == 0)
                throw new ArgumentException("At least one timeout must be specified.", nameof(timeouts));

            this.timeouts = timeouts;
            this.tailBehaviour = tailBehaviour;
        }

        public FixedTimeoutsProvider([NotNull] params TimeSpan[] timeouts)
            : this(TailTimeoutBehaviour.UseRemainingBudget, timeouts)
        {
        }

        public TimeSpan GetTimeout(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            if (currentReplicaIndex >= timeouts.Length)
            {
                return tailBehaviour == TailTimeoutBehaviour.UseRemainingBudget
                    ? budget.Remaining
                    : TimeSpanExtensions.Min(timeouts.Last(), budget.Remaining);
            }

            return TimeSpanExtensions.Min(timeouts[currentReplicaIndex], budget.Remaining);
        }

        public override string ToString()
        {
            return "fixed";
        }
    }
}
