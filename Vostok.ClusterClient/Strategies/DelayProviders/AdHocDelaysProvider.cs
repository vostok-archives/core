using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Strategies.DelayProviders
{
    /// <summary>
    /// Represents a delay provider which issues delays using a fixed set of external delegates.
    /// </summary>
    public class AdHocDelaysProvider : IForkingDelaysProvider
    {
        private readonly Func<TimeSpan>[] providers;
        private readonly TailDelayBehaviour tailBehaviour;

        public AdHocDelaysProvider(TailDelayBehaviour tailBehaviour, [NotNull] params Func<TimeSpan>[] providers)
        {
            if (providers == null)
                throw new ArgumentNullException(nameof(providers));

            if (providers.Length == 0)
                throw new ArgumentException("At least one delay provider delegate must be specified.", nameof(providers));

            this.providers = providers;
            this.tailBehaviour = tailBehaviour;
        }

        public TimeSpan? GetForkingDelay(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            if (currentReplicaIndex < providers.Length)
                return providers[currentReplicaIndex]();

            switch (tailBehaviour)
            {
                case TailDelayBehaviour.RepeatLastValue:
                    return providers.Last()();

                case TailDelayBehaviour.RepeatAllValues:
                    return providers[currentReplicaIndex%providers.Length]();

                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return "ad-hoc";
        }
    }
}
