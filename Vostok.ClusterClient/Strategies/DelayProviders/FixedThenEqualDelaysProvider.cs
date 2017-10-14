using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Strategies.DelayProviders
{
    /// <summary>
    /// Represents a timeout provider that combines a <see cref="FixedDelaysProvider"/> for first few requests and uses an <see cref="EqualDelaysProvider"/> for the rest of them.
    /// </summary>
    public class FixedThenEqualDelaysProvider : IForkingDelaysProvider
    {
        private readonly FixedDelaysProvider fixedProvider;
        private readonly EqualDelaysProvider equalProvider;
        private readonly int fixedDelaysCount;

        public FixedThenEqualDelaysProvider(int tailDivisionFactor, [NotNull] params TimeSpan[] firstDelays)
        {
            equalProvider = new EqualDelaysProvider(tailDivisionFactor);
            fixedProvider = new FixedDelaysProvider(TailDelayBehaviour.StopIssuingDelays, firstDelays);
            fixedDelaysCount = firstDelays.Length;
        }

        public TimeSpan? GetForkingDelay(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            return currentReplicaIndex < fixedDelaysCount
                ? fixedProvider.GetForkingDelay(request, budget, currentReplicaIndex, totalReplicas)
                : equalProvider.GetForkingDelay(request, budget, currentReplicaIndex, totalReplicas);
        }

        public override string ToString()
        {
            return $"{fixedProvider} + {equalProvider}";
        }
    }
}
