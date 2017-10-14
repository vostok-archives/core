using System.Collections.Generic;
using Vostok.Clusterclient.Ordering.Weighed;

namespace Vostok.Clusterclient.Ordering
{
    /// <summary>
    /// Represents an ordering which returns replicas in random order.
    /// </summary>
    public class RandomReplicaOrdering : WeighedReplicaOrdering
    {
        public RandomReplicaOrdering()
            : base (new List<IReplicaWeightModifier>())
        {
        }
    }
}
