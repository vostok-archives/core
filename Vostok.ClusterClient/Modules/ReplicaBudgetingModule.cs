using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    /// <summary>
    /// A module which limits replicas used per request to maintain sliding 'used-replicas/requests' ratio below given threshold.
    /// </summary>
    internal class ReplicaBudgetingModule : IRequestModule
    {
        private static readonly ConcurrentDictionary<string, Counter> counters = new ConcurrentDictionary<string, Counter>();
        private static readonly Stopwatch watch = Stopwatch.StartNew();

        private readonly ReplicaBudgetingOptions options;
        private readonly Func<string, Counter> counterFactory;

        public ReplicaBudgetingModule(ReplicaBudgetingOptions options)
        {
            this.options = options;
            counterFactory = _ => new Counter(options.MinutesToTrack);
        }

        public int Requests => GetCounter().GetMetrics().Requests;

        public int Replicas => GetCounter().GetMetrics().Replicas;

        public double Ratio => ComputeRatio(GetCounter().GetMetrics());

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            var counter = GetCounter();

            counter.AddRequest();

            double ratio;

            var metrics = counter.GetMetrics();
            if (metrics.Requests >= options.MinimumRequests &&
                (ratio = ComputeRatio(counter.GetMetrics())) >= options.CriticalRatio)
            {
                LogLimitingReplicasToUse(context, ratio);
                context.MaximumReplicasToUse = 1;
            }

            var result = await next(context).ConfigureAwait(false);

            counter.AddReplicas(result.ReplicaResults.Count);

            return result;
        }

        private Counter GetCounter()
        {
            return counters.GetOrAdd(options.StorageKey, counterFactory);
        }

        private static double ComputeRatio(CounterMetrics metrics)
        {
            return 1.0 * metrics.Replicas / Math.Max(1.0, metrics.Requests);
        }

        #region Logging

        private void LogLimitingReplicasToUse(IRequestContext context, double ratio)
        {
            context.Log.Warn($"Limiting max used replicas for request to 1 due to current replicas/requests ratio = {ratio:F3}");
        }

        #endregion

        #region CounterMetrics

        private struct CounterMetrics
        {
            public int Requests;
            public int Replicas;
        }

        #endregion

        #region CounterBucket

        private class CounterBucket
        {
            public volatile int Minute;
            public int Requests;
            public int Replicas;
        }

        #endregion

        #region Counter

        private class Counter
        {
            private readonly CounterBucket[] buckets;

            public Counter(int buckets)
            {
                this.buckets = new CounterBucket[buckets];

                for (var i = 0; i < buckets; i++)
                    this.buckets[i] = new CounterBucket();
            }

            public CounterMetrics GetMetrics()
            {
                var metrics = new CounterMetrics();
                var minute = GetCurrentMinute();

                foreach (var bucket in buckets)
                {
                    if (bucket.Minute <= minute - buckets.Length)
                        continue;

                    metrics.Requests += bucket.Requests;
                    metrics.Replicas += bucket.Replicas;
                }

                return metrics;
            }

            public void AddRequest()
            {
                Interlocked.Increment(ref ObtainBucket().Requests);
            }

            public void AddReplicas(int count)
            {
                Interlocked.Add(ref ObtainBucket().Replicas, count);
            }

            private CounterBucket ObtainBucket()
            {
                var minute = GetCurrentMinute();
                var bucketIndex = minute % buckets.Length;

                while (true)
                {
                    var currentBucket = buckets[bucketIndex];
                    if (currentBucket.Minute >= minute)
                        return currentBucket;

                    Interlocked.CompareExchange(ref buckets[bucketIndex], new CounterBucket { Minute = minute }, currentBucket);
                }
            }

            private static int GetCurrentMinute()
            {
                return (int) Math.Floor(watch.Elapsed.TotalMinutes);
            }
        }

        #endregion
    }
}
