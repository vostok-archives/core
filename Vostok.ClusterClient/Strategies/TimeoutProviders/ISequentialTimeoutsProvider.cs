using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Strategies.TimeoutProviders
{
    public interface ISequentialTimeoutsProvider
    {
        /// <summary>
        /// <para>Returns a timeout for next request issued by <see cref="SequentialRequestStrategy"/>.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <param name="budget">Request time budget. Use <see cref="IRequestTimeBudget.Remaining"/> to get remaining time.</param>
        /// <param name="currentReplicaIndex">Zero-based index of current replica.</param>
        /// <param name="totalReplicas">Total count of replicas.</param>
        /// <returns></returns>
        [Pure]
        TimeSpan GetTimeout([NotNull] Request request, [NotNull] IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas);
    }
}
