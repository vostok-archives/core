using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Strategies.DelayProviders
{
    /// <summary>
    /// Represents a delay provider that combines a <see cref="AdHocDelaysProvider"/> for first few requests and uses an <see cref="EqualDelaysProvider"/> for the rest of them.
    /// </summary>
    public class AdHocThenEqualDelaysProvider : IForkingDelaysProvider
    {
        private readonly AdHocDelaysProvider adHocProvider;
        private readonly EqualDelaysProvider equalProvider;
        private readonly int fixedDelaysCount;

        public AdHocThenEqualDelaysProvider(int tailDivisionFactor, [NotNull] params Func<TimeSpan>[] firstDelays)
        {
            equalProvider = new EqualDelaysProvider(tailDivisionFactor);
            adHocProvider = new AdHocDelaysProvider(TailDelayBehaviour.StopIssuingDelays, firstDelays);
            fixedDelaysCount = firstDelays.Length;
        }

        public TimeSpan? GetForkingDelay(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            return currentReplicaIndex < fixedDelaysCount
                ? adHocProvider.GetForkingDelay(request, budget, currentReplicaIndex, totalReplicas)
                : equalProvider.GetForkingDelay(request, budget, currentReplicaIndex, totalReplicas);
        }

        public override string ToString()
        {
            return $"{adHocProvider} + {equalProvider}";
        }
    }
}
