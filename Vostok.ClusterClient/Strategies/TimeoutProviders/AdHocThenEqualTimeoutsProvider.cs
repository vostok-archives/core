using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Strategies.TimeoutProviders
{
    /// <summary>
    /// Represents a timeout provider that combines a <see cref="AdHocTimeoutsProvider"/> for first few requests and uses an <see cref="EqualTimeoutsProvider"/> for the rest of them.
    /// </summary>
    public class AdHocThenEqualTimeoutsProvider : ISequentialTimeoutsProvider
    {
        private readonly AdHocTimeoutsProvider adHocProvider;
        private readonly EqualTimeoutsProvider equalProvider;
        private readonly int fixedTimeoutsCount;

        public AdHocThenEqualTimeoutsProvider(int tailDivisionFactor, [NotNull] params Func<TimeSpan>[] firstTimeouts)
        {
            equalProvider = new EqualTimeoutsProvider(tailDivisionFactor);
            adHocProvider = new AdHocTimeoutsProvider(firstTimeouts);
            fixedTimeoutsCount = firstTimeouts.Length;
        }

        public TimeSpan GetTimeout(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            return currentReplicaIndex < fixedTimeoutsCount
                ? adHocProvider.GetTimeout(request, budget, currentReplicaIndex, totalReplicas)
                : equalProvider.GetTimeout(request, budget, currentReplicaIndex - fixedTimeoutsCount, totalReplicas);
        }

        public override string ToString()
        {
            return $"{adHocProvider} + {equalProvider}";
        }
    }
}
