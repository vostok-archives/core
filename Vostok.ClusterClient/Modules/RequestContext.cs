using System.Collections.Generic;
using System.Threading;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies;
using Vostok.Commons.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    internal class RequestContext : IRequestContext
    {
        private List<ReplicaResult> results;
        private readonly object resultsLock = new object();

        public RequestContext(
            Request request, 
            IRequestStrategy strategy, 
            IRequestTimeBudget budget, 
            ILog log, 
            CancellationToken cancellationToken, 
            RequestPriority? priority,
            int maximumReplicasToUse)
        {
            Request = request;
            Strategy = strategy;
            Budget = budget;
            Log = log;
            Priority = priority;
            CancellationToken = cancellationToken;
            MaximumReplicasToUse = maximumReplicasToUse;

            ResetReplicaResults();
        }

        public Request Request { get; set; }

        public IRequestStrategy Strategy { get; }

        public IRequestTimeBudget Budget { get; }

        public ILog Log { get; }

        public CancellationToken CancellationToken { get; }

        public RequestPriority? Priority { get; }

        public int MaximumReplicasToUse { get; set; }

        public void SetReplicaResult(ReplicaResult result)
        {
            lock (resultsLock)
            {
                if (results == null)
                    return;

                for (var i = 0; i < results.Count; i++)
                {
                    if (results[i].Replica.Equals(result.Replica))
                    {
                        results[i] = result;
                        return;
                    }
                }

                results.Add(result);
            }
        }

        public List<ReplicaResult> FreezeReplicaResults()
        {
            lock (resultsLock)
            {
                var currentResults = results;

                results = null;

                return currentResults;
            }
        }

        public void ResetReplicaResults()
        {
            lock (resultsLock)
            {
                results = new List<ReplicaResult>(2);
            }
        }
    }
}
