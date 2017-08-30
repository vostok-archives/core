using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;

namespace Vostok.Clusterclient.Ordering.Weighed
{
    /// <summary>
    /// <para>Represents a modifier used to manipulate replica weights in <see cref="WeighedReplicaOrdering"/>.</para>
    /// <para>Modifiers form a chain where each one sees the weight already modified by all previous modifiers.</para>
    /// </summary>
    public interface IReplicaWeightModifier
    {
        /// <summary>
        /// <para>Modifies current <paramref name="weight"/> of given <paramref name="replica"/> for given <paramref name="request"/></para>
        /// <para>Implementations may use <paramref name="storageProvider"/> to fetch previously stored information about replicas.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        void Modify(
            [NotNull] Uri replica,
            [NotNull] IList<Uri> allReplicas,
            [NotNull] IReplicaStorageProvider storageProvider,
            [NotNull] Request request,
            ref double weight);

        /// <summary>
        /// <para>Receives feedback via <see cref="ReplicaResult"/> obtained while sending request.</para>
        /// <para>Implementations may use this data to improve ordering quality in response to changing environment.</para>
        /// <para>Implementations may use <paramref name="storageProvider"/> to fetch/store information about replicas.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        void Learn(
            [NotNull] ReplicaResult result,
            [NotNull] IReplicaStorageProvider storageProvider);
    }
}
