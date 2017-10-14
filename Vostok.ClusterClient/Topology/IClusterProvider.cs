using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Topology
{
    public interface IClusterProvider
    {
        /// <summary>
        /// <para>Returns a list of replica urls to use for cluster communication.</para>
        /// <para>May return null or empty list if no replicas are known.</para>
        /// <para>Implementations should take care to cache the result for optimal performance.</para>
        /// <para>Implementations should not try to reorder replicas (this is a responsibility of <see cref="Ordering.IReplicaOrdering"/>).</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        [Pure]
        [CanBeNull]
        [ItemNotNull]
        IList<Uri> GetCluster();
    }
}
