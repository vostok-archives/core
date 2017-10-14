using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;

namespace Vostok.Clusterclient.Strategies
{
    /// <summary>
    /// Represents a strategy which maintains several parallel requests right from the start and stops at any result with <see cref="ResponseVerdict.Accept"/> verdict.
    /// </summary>
    /// <example>
    /// Example of execution with parallellism = 3:
    /// <code>
    /// o-------------- (replica1) -------------------------------->
    /// o-------------- (replica2) ----X o------ (replica4) ------->
    /// o-------------- (replica3) ------------------------------- V (success)
    /// </code>
    /// <code>
    /// -------------------------------------------------------------------------------> (time)
    ///                                ↑ failure(replica2)         ↑ success(replica3)
    /// </code>
    /// </example>
    public class ParallelRequestStrategy : IRequestStrategy
    {
        private readonly int parallelismLevel;

        public ParallelRequestStrategy(int parallelismLevel)
        {
            if (parallelismLevel <= 0)
                throw new ArgumentOutOfRangeException(nameof(parallelismLevel), "Parallelism level must be a positive number.");

            this.parallelismLevel = parallelismLevel;
        }

        public async Task SendAsync(Request request, IRequestSender sender, IRequestTimeBudget budget, IEnumerable<Uri> replicas, int replicasCount, CancellationToken cancellationToken)
        {
            var initialRequestCount = Math.Min(parallelismLevel, replicasCount);
            var currentTasks = new List<Task<ReplicaResult>>(initialRequestCount);

            using (var localCancellationSource = new CancellationTokenSource())
            using (var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, localCancellationSource.Token))
            {
                var linkedCancellationToken = linkedCancellationSource.Token;

                using (var replicasEnumerator = replicas.GetEnumerator())
                {
                    for (var i = 0; i < initialRequestCount; i++)
                    {
                        if (!replicasEnumerator.MoveNext())
                            throw new InvalidOperationException("Replicas enumerator ended prematurely. This is definitely a bug in code.");

                        currentTasks.Add(sender.SendToReplicaAsync(replicasEnumerator.Current, request, budget.Remaining, linkedCancellationToken));
                    }

                    while (currentTasks.Count > 0)
                    {
                        var completedTask = await Task.WhenAny(currentTasks).ConfigureAwait(false);

                        currentTasks.Remove(completedTask);

                        var completedResult = await completedTask.ConfigureAwait(false);
                        if (completedResult.Verdict == ResponseVerdict.Accept)
                        {
                            localCancellationSource.Cancel();
                            return;
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        if (replicasEnumerator.MoveNext())
                            currentTasks.Add(sender.SendToReplicaAsync(replicasEnumerator.Current, request, budget.Remaining, linkedCancellationToken));
                    }
                }
            }
        }

        public override string ToString()
        {
            return "Parallel-" + parallelismLevel;
        }
    }
}
