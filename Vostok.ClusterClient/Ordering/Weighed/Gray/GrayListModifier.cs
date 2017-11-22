using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Clusterclient.Ordering.Weighed.Gray
{
    /// <summary>
    /// <para>Represents a modifier which keeps a list of bad ("gray") replicas. Replicas which are not in this list are called "white".</para>
    /// <para>The weight of white replicas is not modified at all.</para>
    /// <para>The weight of gray replicas is dropped to zero.</para>
    /// <para>A replica is added to gray list when it returns a response with <see cref="ResponseVerdict.Reject"/> verdict.</para>
    /// <para>A replica remains in gray list for a period of time known as gray period, as given by the <see cref="IGrayPeriodProvider"/> implementation.</para>
    /// </summary>
    public class GrayListModifier : IReplicaWeightModifier
    {
        private static readonly string storageKey = typeof (GrayListModifier).FullName;

        private readonly IGrayPeriodProvider grayPeriodProvider;
        private readonly ITimeProvider timeProvider;
        private readonly ILog log;

        public GrayListModifier([NotNull] IGrayPeriodProvider grayPeriodProvider, [CanBeNull] ILog log)
            : this(grayPeriodProvider, new TimeProvider(), log)
        {
        }

        public GrayListModifier(TimeSpan grayPeriod, [CanBeNull] ILog log)
            : this(new FixedGrayPeriodProvider(grayPeriod), log)
        {
        }

        internal GrayListModifier([NotNull] IGrayPeriodProvider grayPeriodProvider, [NotNull] ITimeProvider timeProvider, [CanBeNull] ILog log)
        {
            if (grayPeriodProvider == null)
                throw new ArgumentNullException(nameof(grayPeriodProvider));

            if (timeProvider == null)
                throw new ArgumentNullException(nameof(timeProvider));

            this.grayPeriodProvider = grayPeriodProvider;
            this.timeProvider = timeProvider;
            this.log = log ?? new SilentLog();
        }

        public void Modify(Uri replica, IList<Uri> allReplicas, IReplicaStorageProvider storageProvider, Request request, ref double weight)
        {
            DateTime lastGrayTimestamp;

            var storage = storageProvider.Obtain<DateTime>(storageKey);
            if (!storage.TryGetValue(replica, out lastGrayTimestamp))
                return;

            var currentTime = timeProvider.GetCurrentTime();
            var grayPeriod = grayPeriodProvider.GetGrayPeriod();

            if (lastGrayTimestamp + grayPeriod >= currentTime)
            {
                weight = 0.0;
            }
            else
            {
                if (storage.Remove(replica, lastGrayTimestamp))
                    LogReplicaIsNoLongerGray(replica);
            }
        }

        public void Learn(ReplicaResult result, IReplicaStorageProvider storageProvider)
        {
            if (result.Verdict != ResponseVerdict.Reject)
                return;

            var storage = storageProvider.Obtain<DateTime>(storageKey);
            var wasGray = storage.ContainsKey(result.Replica);

            storage[result.Replica] = timeProvider.GetCurrentTime();

            if (!wasGray)
                LogReplicaIsGrayNow(result.Replica);
        }

        private void LogReplicaIsGrayNow(Uri replica)
        {
            log.Warn($"Replica '{replica}' is now gray.");
        }

        private void LogReplicaIsNoLongerGray(Uri replica)
        {
            log.Info($"Replica '{replica}' is no longer gray.");
        }
    }
}
