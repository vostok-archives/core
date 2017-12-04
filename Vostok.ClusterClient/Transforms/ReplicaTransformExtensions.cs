using System;
using System.Collections.Generic;

namespace Vostok.Clusterclient.Transforms
{
    public static class ReplicaTransformExtensions
    {
        public static IList<Uri> Transform(this IReplicaTransform transform, IList<Uri> replicas)
        {
            var transformed = new Uri[replicas.Count];

            for (var i = 0; i < replicas.Count; i++)
            {
                transformed[i] = transform.Transform(replicas[i]);
            }

            return transformed;
        }
    }
}
