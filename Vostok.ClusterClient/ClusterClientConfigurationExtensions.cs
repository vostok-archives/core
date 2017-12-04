using System;
using System.Collections.Generic;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Ordering.Weighed;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;

namespace Vostok.Clusterclient
{
    public static class ClusterClientConfigurationExtensions
    {
        /// <summary>
        /// Initializes configuration's <see cref="IClusterClientConfiguration.ReplicaOrdering"/> with a <see cref="WeighedReplicaOrdering"/> built with a given delegate acting on a <see cref="IWeighedReplicaOrderingBuilder"/> instance.
        /// </summary>
        public static void SetupWeighedReplicaOrdering(this IClusterClientConfiguration configuration, Action<IWeighedReplicaOrderingBuilder> build)
        {
            var builder = new WeighedReplicaOrderingBuilder(configuration.Log);

            build(builder);

            configuration.ReplicaOrdering = builder.Build();
        }

        /// <summary>
        /// Initializes configuration's <see cref="IClusterClientConfiguration.ResponseCriteria"/> list with given <paramref name="criteria"/>.
        /// </summary>
        public static void SetupResponseCriteria(this IClusterClientConfiguration configuration, params IResponseCriterion[] criteria)
        {
            configuration.ResponseCriteria = new List<IResponseCriterion>(criteria);
        }

        /// <summary>
        /// Adds given <paramref name="module"/> to configuration's <see cref="IClusterClientConfiguration.Modules"/> list.
        /// </summary>
        public static void AddRequestModule(this IClusterClientConfiguration configuration, IRequestModule module)
        {
            (configuration.Modules ?? (configuration.Modules = new List<IRequestModule>())).Add(module);
        }

        /// <summary>
        /// Adds given <paramref name="transform"/> to configuration's <see cref="IClusterClientConfiguration.RequestTransforms"/> list.
        /// </summary>
        public static void AddRequestTransform(this IClusterClientConfiguration configuration, IRequestTransform transform)
        {
            (configuration.RequestTransforms ?? (configuration.RequestTransforms = new List<IRequestTransform>())).Add(transform);
        }

        /// <summary>
        /// Adds an <see cref="AdHocRequestTransform"/> with given <paramref name="transform"/> function to configuration's <see cref="IClusterClientConfiguration.RequestTransforms"/> list.
        /// </summary>
        public static void AddRequestTransform(this IClusterClientConfiguration configuration, Func<Request, Request> transform)
        {
            AddRequestTransform(configuration, new AdHocRequestTransform(transform));
        }

        /// <summary>
        /// Adds given <paramref name="transform"/> to configuration's <see cref="IClusterClientConfiguration.ResponseTransforms"/> list.
        /// </summary>
        public static void AddResponseTransform(this IClusterClientConfiguration configuration, IResponseTransform transform)
        {
            (configuration.ResponseTransforms ?? (configuration.ResponseTransforms = new List<IResponseTransform>())).Add(transform);
        }

        /// <summary>
        /// Adds an <see cref="AdHocResponseTransform"/> with given <paramref name="transform"/> function to configuration's <see cref="IClusterClientConfiguration.ResponseTransforms"/> list.
        /// </summary>
        public static void AddResponseTransform(this IClusterClientConfiguration configuration, Func<Response, Response> transform)
        {
            AddResponseTransform(configuration, new AdHocResponseTransform(transform));
        }

        /// <summary>
        /// Modifies configuration's <see cref="IClusterClientConfiguration.ClusterProvider"/> to repeat all of its replicas <paramref name="repeatCount"/> times.
        /// </summary>
        public static void RepeatReplicas(this IClusterClientConfiguration configuration, int repeatCount)
        {
            if (configuration.ClusterProvider == null)
                return;

            configuration.ClusterProvider = new RepeatingClusterProvider(configuration.ClusterProvider, repeatCount);
        }

        /// <summary>
        /// Sets up an adaptive client throttling mechanism with given parameters.
        /// </summary>
        /// <param name="configuration">A configuration to be modified.</param>
        /// <param name="storageKey">See <see cref="AdaptiveThrottlingOptions.StorageKey"/>.</param>
        /// <param name="minutesToTrack">See <see cref="AdaptiveThrottlingOptions.MinutesToTrack"/>.</param>
        /// <param name="minimumRequests">See <see cref="AdaptiveThrottlingOptions.MinimumRequests"/>.</param>
        /// <param name="criticalRatio">See <see cref="AdaptiveThrottlingOptions.CriticalRatio"/>.</param>
        /// <param name="maximumRejectProbability">See <see cref="AdaptiveThrottlingOptions.MaximumRejectProbability"/>.</param>
        public static void SetupAdaptiveThrottling(
            this IClusterClientConfiguration configuration,
            string storageKey,
            int minutesToTrack = ClusterClientDefaults.AdaptiveThrottlingMinutesToTrack,
            int minimumRequests = ClusterClientDefaults.AdaptiveThrottlingMinimumRequests,
            double criticalRatio = ClusterClientDefaults.AdaptiveThrottlingCriticalRatio,
            double maximumRejectProbability = ClusterClientDefaults.AdaptiveThrottlingRejectProbabilityCap)
        {
            configuration.AdaptiveThrottling = new AdaptiveThrottlingOptions(storageKey, minutesToTrack, minimumRequests, criticalRatio, maximumRejectProbability);
        }

        /// <summary>
        /// Sets up a replica budgeting mechanism with given parameters.
        /// </summary>
        /// <param name="configuration">A configuration to be modified.</param>
        /// <param name="storageKey">See <see cref="ReplicaBudgetingOptions.StorageKey"/>.</param>
        /// <param name="minutesToTrack">See <see cref="ReplicaBudgetingOptions.MinutesToTrack"/>.</param>
        /// <param name="minimumRequests">See <see cref="ReplicaBudgetingOptions.MinimumRequests"/>.</param>
        /// <param name="criticalRatio">See <see cref="ReplicaBudgetingOptions.CriticalRatio"/>.</param>
        public static void SetupReplicaBudgeting(
            this IClusterClientConfiguration configuration,
            string storageKey,
            int minutesToTrack = ClusterClientDefaults.ReplicaBudgetingMinutesToTrack,
            int minimumRequests = ClusterClientDefaults.ReplicaBudgetingMinimumRequests,
            double criticalRatio = ClusterClientDefaults.ReplicaBudgetingCriticalRatio)
        {
            configuration.ReplicaBudgeting = new ReplicaBudgetingOptions(storageKey, minutesToTrack, minimumRequests, criticalRatio);
        }
    }
}
