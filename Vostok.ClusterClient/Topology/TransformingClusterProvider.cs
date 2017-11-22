using System;
using System.Collections.Generic;
using System.Threading;
using Vostok.Clusterclient.Transforms;

namespace Vostok.Clusterclient.Topology
{
    internal class TransformingClusterProvider : IClusterProvider
    {
        private readonly IClusterProvider provider;
        private readonly IReplicaTransform transform;

        private Tuple<IList<Uri>, IList<Uri>> cache;

        public TransformingClusterProvider(IClusterProvider provider, IReplicaTransform transform)
        {
            this.provider = provider;
            this.transform = transform;
        }

        public IList<Uri> GetCluster()
        {
            var currentReplicas = provider.GetCluster();
            if (currentReplicas == null || currentReplicas.Count == 0)
                return currentReplicas;

            var currentCache = cache;

            if (currentCache != null && ReferenceEquals(currentCache.Item1, currentReplicas))
                return currentCache.Item2;

            var transformedReplicas = transform.Transform(currentReplicas);

            Interlocked.CompareExchange(ref cache, Tuple.Create(currentReplicas, transformedReplicas), currentCache);

            return transformedReplicas;
        }
    }
}
