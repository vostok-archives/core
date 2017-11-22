using System;
using System.Collections.Generic;
using System.Threading;

namespace Vostok.Clusterclient.Topology
{
    internal class RepeatingClusterProvider : IClusterProvider
    {
        private readonly IClusterProvider provider;
        private readonly int repeatCount;

        private Tuple<IList<Uri>, IList<Uri>> cache;

        public RepeatingClusterProvider(IClusterProvider provider, int repeatCount)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (repeatCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(repeatCount), "Repeat count must be positive.");

            this.provider = provider;
            this.repeatCount = repeatCount;
        }

        public IList<Uri> GetCluster()
        {
            var currentReplicas = provider.GetCluster();
            if (currentReplicas == null || currentReplicas.Count == 0)
                return currentReplicas;

            var currentCache = cache;

            if (currentCache != null && ReferenceEquals(currentCache.Item1, currentReplicas))
                return currentCache.Item2;

            var repeatedReplicas = Repeat(currentReplicas, repeatCount);

            Interlocked.CompareExchange(ref cache, Tuple.Create(currentReplicas, repeatedReplicas), currentCache);

            return repeatedReplicas;
        }

        private static IList<Uri> Repeat(IList<Uri> currentReplicas, int repeatCount)
        {
            var repeatedReplicas = new List<Uri>(currentReplicas.Count*repeatCount);

            for (var i = 0; i < repeatCount; i++)
            {
                repeatedReplicas.AddRange(currentReplicas);
            }

            return repeatedReplicas;
        }
    }
}
