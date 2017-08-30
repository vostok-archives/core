using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Strategies.DelayProviders
{
    public interface IForkingDelaysProvider
    {
        /// <summary>
        /// <para>Returns a forking delay for next request issued by <see cref="ForkingRequestStrategy"/>.</para>
        /// <para>Returning a null value prevents any forking from being planned.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        /// <param name="request">Request to be sent.</param>
        /// <param name="budget">Request time budget. Use <see cref="IRequestTimeBudget.Remaining"/> to get remaining time.</param>
        /// <param name="currentReplicaIndex">Zero-based index of current replica.</param>
        /// <param name="totalReplicas">Total count of replicas.</param>
        [Pure]
        TimeSpan? GetForkingDelay([NotNull] Request request, [NotNull] IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas);
    }
}
