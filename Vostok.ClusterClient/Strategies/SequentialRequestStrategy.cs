using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;
using Vostok.Clusterclient.Strategies.TimeoutProviders;
using Vostok.Commons.Utilities;

namespace Vostok.Clusterclient.Strategies
{
    /// <summary>
    /// <para>Represents a strategy which traverses replicas sequentially, does not use parallelism and stops at any result with <see cref="ResponseVerdict.Accept"/> verdict.</para>
    /// <para>Request timeouts for each attempt are given by an implementation of <see cref="ISequentialTimeoutsProvider"/> interface.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// o--------X (replica1) o--------------X (replica2) o----------> V (replica3)
    /// </code>
    /// <code>
    /// ------------------------------------------------------------------------------------> (time)
    ///          ↑ failure(replica1)         ↑ failure(replica2)       ↑ success(replica3)
    /// </code>
    /// </example>
    public class SequentialRequestStrategy : IRequestStrategy
    {
        private readonly ISequentialTimeoutsProvider timeoutsProvider;

        public SequentialRequestStrategy([NotNull] ISequentialTimeoutsProvider timeoutsProvider)
        {
            if (timeoutsProvider == null)
                throw new ArgumentNullException(nameof(timeoutsProvider));

            this.timeoutsProvider = timeoutsProvider;
        }

        public async Task SendAsync(Request request, IRequestSender sender, IRequestTimeBudget budget, IEnumerable<Uri> replicas, int replicasCount, CancellationToken cancellationToken)
        {
            var currentReplicaIndex = 0;

            foreach (var replica in replicas)
            {
                if (budget.HasExpired)
                    break;

                var timeout = TimeSpanExtensions.Min(timeoutsProvider.GetTimeout(request, budget, currentReplicaIndex++, replicasCount), budget.Remaining);

                var result = await sender.SendToReplicaAsync(replica, request, timeout, cancellationToken).ConfigureAwait(false);
                if (result.Verdict == ResponseVerdict.Accept)
                    break;

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public override string ToString()
        {
            return $"Sequential({timeoutsProvider})";
        }
    }
}
