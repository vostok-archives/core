using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;

namespace Vostok.Clusterclient.Strategies
{
    /// <summary>
    /// Represents a strategy which only sends a request to a single, first replica, using all available time budget.
    /// </summary>
    /// <example>
    /// <code>
    /// o--------------------- (replica) -----------> X (failure)
    /// o--------------------- (replica) -----------> V (success)
    /// </code>
    /// </example>
    public class SingleReplicaRequestStrategy : IRequestStrategy
    {
        public Task SendAsync(Request request, IRequestSender sender, IRequestTimeBudget budget, IEnumerable<Uri> replicas, int replicasCount, CancellationToken cancellationToken)
        {
            return sender.SendToReplicaAsync(replicas.First(), request, budget.Remaining, cancellationToken);
        }

        public override string ToString()
        {
            return "SingleReplica";
        }
    }
}
