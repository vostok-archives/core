using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Utilities;

namespace Vostok.Clusterclient.Strategies.TimeoutProviders
{
    /// <summary>
    /// Represents a timeout provider which issues timeouts using a fixed set of external delegates.
    /// </summary>
    public class AdHocTimeoutsProvider : ISequentialTimeoutsProvider
    {
        private readonly Func<TimeSpan>[] providers;
        private readonly TailTimeoutBehaviour tailBehaviour;

        public AdHocTimeoutsProvider(TailTimeoutBehaviour tailBehaviour, [NotNull] params Func<TimeSpan>[] providers)
        {
            if (providers == null)
                throw new ArgumentNullException(nameof(providers));

            if (providers.Length == 0)
                throw new ArgumentException("At least one timeout provider delegate must be specified.", nameof(providers));

            this.providers = providers;
            this.tailBehaviour = tailBehaviour;
        }

        public AdHocTimeoutsProvider([NotNull] params Func<TimeSpan>[] providers)
            : this(TailTimeoutBehaviour.UseRemainingBudget, providers)
        {
        }

        public TimeSpan GetTimeout(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            if (currentReplicaIndex >= providers.Length)
            {
                return tailBehaviour == TailTimeoutBehaviour.UseRemainingBudget
                    ? budget.Remaining
                    : TimeSpanExtensions.Min(providers.Last()(), budget.Remaining);
            }

            return TimeSpanExtensions.Min(providers[currentReplicaIndex](), budget.Remaining);
        }

        public override string ToString()
        {
            return "ad-hoc";
        }
    }
}
