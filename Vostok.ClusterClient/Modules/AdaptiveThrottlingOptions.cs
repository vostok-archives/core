using System;

namespace Vostok.Clusterclient.Modules
{
    public class AdaptiveThrottlingOptions
    {
        public AdaptiveThrottlingOptions(
            string storageKey, 
            int minutesToTrack = ClusterClientDefaults.AdaptiveThrottlingMinutesToTrack, 
            int minimumRequests = ClusterClientDefaults.AdaptiveThrottlingMinimumRequests, 
            double criticalRatio = ClusterClientDefaults.AdaptiveThrottlingCriticalRatio, 
            double maximumRejectProbability = ClusterClientDefaults.AdaptiveThrottlingRejectProbabilityCap)
        {
            if (storageKey == null)
                throw new ArgumentNullException(nameof(storageKey));

            if (minutesToTrack < 1)
                throw new ArgumentOutOfRangeException(nameof(minutesToTrack), "Minutes to track parameter must be >= 1.");

            if (criticalRatio <= 1.0)
                throw new ArgumentOutOfRangeException(nameof(criticalRatio), "Critical ratio must be in (1; +inf) range.");

            if (maximumRejectProbability < 0.0 || maximumRejectProbability > 1.0)
                throw new ArgumentOutOfRangeException(nameof(maximumRejectProbability), "Maximum rejection probability must be in [0; 1] range.");

            StorageKey = storageKey;
            MinutesToTrack = minutesToTrack;
            MinimumRequests = minimumRequests;
            CriticalRatio = criticalRatio;
            MaximumRejectProbability = maximumRejectProbability;
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
        /// A minimum requests count in <see cref="MinutesToTrack"/> minutes to reject any request.
        /// </summary>
        public int MinimumRequests { get; }

        /// <summary>
        /// A minimum ratio of requests to accepts eligible for rejection. Must be > 1.
        /// </summary>
        public double CriticalRatio { get; }

        /// <summary>
        /// A cap on the request rejection probability to prevent eternal rejection.
        /// </summary>
        public double MaximumRejectProbability { get; }
    }
}