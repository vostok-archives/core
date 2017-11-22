using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    /// <summary>
    /// An implementation of adaptive client throttling mechanism described in https://landing.google.com/sre/book/chapters/handling-overload.html.
    /// </summary>
    internal class AdaptiveThrottlingModule : IRequestModule
    {
        private static readonly ConcurrentDictionary<string, Counter> counters = new ConcurrentDictionary<string, Counter>();
        private static readonly Stopwatch watch = Stopwatch.StartNew();

        private readonly AdaptiveThrottlingOptions options;
        private readonly Func<string, Counter> counterFactory;

        public AdaptiveThrottlingModule(AdaptiveThrottlingOptions options)
        {
            this.options = options;
            counterFactory = _ => new Counter(options.MinutesToTrack);
        }

        public int Requests => GetCounter().GetMetrics().Requests;

        public int Accepts => GetCounter().GetMetrics().Accepts;

        public double Ratio => ComputeRatio(GetCounter().GetMetrics());

        public double RejectionProbability => ComputeRejectionProbability(GetCounter().GetMetrics(), options);

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            var counter = GetCounter();

            counter.AddRequest();

            double ratio;
            double rejectionProbability;

            var metrics = counter.GetMetrics();
            if (metrics.Requests >= options.MinimumRequests &&
                (ratio = ComputeRatio(metrics)) >= options.CriticalRatio &&
                (rejectionProbability = ComputeRejectionProbability(metrics, options)) > ThreadSafeRandom.NextDouble())
            {
                LogThrottledRequest(context, ratio, rejectionProbability);

                return ClusterResult.Throttled(context.Request);
            }

            var result = await next(context).ConfigureAwait(false);

            UpdateCounter(counter, result);

            return result;
        }

        private Counter GetCounter()
        {
            return counters.GetOrAdd(options.StorageKey, counterFactory);
        }

        private static double ComputeRatio(CounterMetrics metrics)
        {
            return 1.0 * metrics.Requests / Math.Max(1.0, metrics.Accepts);
        }

        private static double ComputeRejectionProbability(CounterMetrics metrics, AdaptiveThrottlingOptions options)
        {
            var probability = 1.0 * (metrics.Requests - options.CriticalRatio * metrics.Accepts) / (metrics.Requests + 1);

            probability = Math.Max(probability, 0.0);
            probability = Math.Min(probability, options.MaximumRejectProbability);

            return probability;
        }

        private static void UpdateCounter(Counter counter, ClusterResult result)
        {
            if (result.ReplicaResults.Any(r => r.Verdict == ResponseVerdict.Accept))
            {
                counter.AddAccept();
            }
        }

        #region Logging

        private void LogThrottledRequest(IRequestContext context, double ratio, double rejectionProbability)
        {
            context.Log.Warn($"Throttled request without sending it. Request/accept ratio = {ratio:F3}. Rejection probability = {rejectionProbability:F3}");
        }

        #endregion

        #region CounterMetrics

        private struct CounterMetrics
        {
            public int Requests;
            public int Accepts;
        }

        #endregion

        #region CounterBucket

        private class CounterBucket
        {
            public volatile int Minute;
            public int Requests;
            public int Accepts;
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
                    metrics.Accepts += bucket.Accepts;
                }

                return metrics;
            }

            public void AddRequest()
            {
                Interlocked.Increment(ref ObtainBucket().Requests);
            }

            public void AddAccept()
            {
                Interlocked.Increment(ref ObtainBucket().Accepts);
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
