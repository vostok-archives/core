using System;

namespace Vostok.Clusterclient.Modules
{
    public class ReplicaBudgetingOptions
    {
        public ReplicaBudgetingOptions(
            string storageKey, 
            int minutesToTrack = ClusterClientDefaults.ReplicaBudgetingMinutesToTrack, 
            int minimumRequests = ClusterClientDefaults.ReplicaBudgetingMinimumRequests, 
            double criticalRatio = ClusterClientDefaults.ReplicaBudgetingCriticalRatio)
        {
            if (storageKey == null)
                throw new ArgumentNullException(nameof(storageKey));

            if (minutesToTrack < 1)
                throw new ArgumentOutOfRangeException(nameof(minutesToTrack), "Minutes to track parameter must be >= 1.");

            if (criticalRatio <= 1.0)
                throw new ArgumentOutOfRangeException(nameof(criticalRatio), "Critical ratio must be in (1; +inf) range.");

            StorageKey = storageKey;
            MinutesToTrack = minutesToTrack;
            MinimumRequests = minimumRequests;
            CriticalRatio = criticalRatio;
        }

        /// <summary>
        /// A key used to decouple statistics for different services.
        /// </summary>
        public string StorageKey { get; }

        /// <summary>
        /// How much minutes of statistics will be tracked.
        /// </summary>
        public int MinutesToTrack { get; }

        /// <summary>
        /// A minimum requests count in <see cref="MinutesToTrack"/> minutes to limit available replicas for request.
        /// </summary>
        public int MinimumRequests { get; }

        /// <summary>
        /// A maximum allowed ratio of used replicas count to issued requests count.
        /// </summary>
        public double CriticalRatio { get; }
    }
}