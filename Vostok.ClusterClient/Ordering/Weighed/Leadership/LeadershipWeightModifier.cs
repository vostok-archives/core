using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Clusterclient.Ordering.Weighed.Leadership
{
    /// <summary>
    /// <para>Represents a modifier which divides all replicas into two categories: leader and reservists.</para>
    /// <para>It assumes that cluster has a single leader and only leader is capable of serving client requests.</para>
    /// <para>Leader weight does not get modified at all.</para>
    /// <para>Reservist weight gets dropped to zero.</para>
    /// <para>Initially all replicas are considered reservists.</para>
    /// <para>A reservist becomes a leader when an implementation of <see cref="ILeaderResultDetector.IsLeaderResult"/> returns <c>true</c> for its response.</para>
    /// <para>A leader becomes a reservist when an implementation of <see cref="ILeaderResultDetector.IsLeaderResult"/> returns <c>false</c> for its response.</para>
    /// </summary>
    public class LeadershipWeightModifier : IReplicaWeightModifier
    {
        private static readonly string storageKey = typeof (LeadershipWeightModifier).FullName;

        private readonly ILeaderResultDetector resultDetector;
        private readonly ILog log;

        public LeadershipWeightModifier(ILeaderResultDetector resultDetector, ILog log)
        {
            if (resultDetector == null)
                throw new ArgumentNullException(nameof(resultDetector));

            this.resultDetector = resultDetector;
            this.log = log ?? new SilentLog();
        }

        public void Modify(Uri replica, IList<Uri> allReplicas, IReplicaStorageProvider storageProvider, Request request, ref double weight)
        {
            if (!IsLeader(replica, storageProvider.Obtain<bool>(storageKey)))
                weight = 0.0;
        }

        public void Learn(ReplicaResult result, IReplicaStorageProvider storageProvider)
        {
            var storage = storageProvider.Obtain<bool>(storageKey);

            bool hadStoredStatus;

            var wasLeader = IsLeader(result.Replica, storage, out hadStoredStatus);
            var isLeader = resultDetector.IsLeaderResult(result);
            if (isLeader == wasLeader)
                return;

            var updatedStatus = hadStoredStatus 
                ? storage.TryUpdate(result.Replica, isLeader, wasLeader) 
                : storage.TryAdd(result.Replica, isLeader);

            if (updatedStatus)
            {
                if (isLeader)
                {
                    LogLeaderDetected(result.Replica);
                }
                else
                {
                    LogLeaderFailed(result.Replica);
                }
            }
        }

        private static bool IsLeader(Uri replica, ConcurrentDictionary<Uri, bool> storage)
        {
            return IsLeader(replica, storage, out _);
        }

        private static bool IsLeader(Uri replica, ConcurrentDictionary<Uri, bool> storage, out bool hadStoredStatus)
        {
            bool isLeader;

            return (hadStoredStatus = storage.TryGetValue(replica, out isLeader)) && isLeader;
        }

        private void LogLeaderDetected(Uri resultReplica)
        {
            log.Info($"Replica '{resultReplica}' is leader now.");
        }

        private void LogLeaderFailed(Uri resultReplica)
        {
            log.Warn($"Replica '{resultReplica}' is no longer considered a leader.");
        }
    }
}
