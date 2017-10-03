using System;
using System.Collections.Generic;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Ordering;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed;
using Vostok.Clusterclient.Retry;
using Vostok.Clusterclient.Strategies;
using Vostok.Logging;

namespace Vostok.Clusterclient
{
    public static class ClusterClientDefaults
    {
        public const double InitialReplicaWeight = 1.0;
        public const double MinimumReplicaWeight = 0.0;
        public const double MaximumReplicaWeight = 10.0;

        public const double AdaptiveHealthMinimumValue = 0.001;
        public const double AdaptiveHealthUpMultiplier = 1.5;
        public const double AdaptiveHealthDownMultiplier = 0.5;

        public const int AdaptiveThrottlingMinimumRequests = 30;
        public const int AdaptiveThrottlingMinutesToTrack = 2;
        public const double AdaptiveThrottlingCriticalRatio = 2.0;
        public const double AdaptiveThrottlingRejectProbabilityCap = 0.8;

        public const int ReplicaBudgetingMinimumRequests = 30;
        public const int ReplicaBudgetingMinutesToTrack = 2;
        public const double ReplicaBudgetingCriticalRatio = 1.2;

        public const bool LogRequestDetails = true;
        public const bool LogResultDetails = true;
        public const bool LogReplicaRequests = true;
        public const bool LogReplicaResults = true;
        public const bool TransferDistributedContext = true;
        public const bool IncludeRequestTimeoutHeader = true;
        public const bool IncludeClientIdentityHeader = true;
        public const bool EnableTracing = true;

        public const int MaxReplicasUsedPerRequest = 3;

        public static readonly ReplicaStorageScope ReplicaStorageScope = ReplicaStorageScope.Process;

        public static readonly IRetryPolicy RetryPolicy = new NeverRetryPolicy();

        public static readonly IRetryStrategy RetryStrategy = new ConstantDelayRetryStrategy(1, TimeSpan.Zero);

        public static readonly IResponseSelector ResponseSelector = new LastAcceptedResponseSelector();

        public static readonly IRequestStrategy RequestStrategy = Strategy.Forking3;

        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        public static readonly TimeSpan AdaptiveHealthDamageDecayDuration = TimeSpan.FromMinutes(10);

        public static List<IResponseCriterion> ResponseCriteria()
        {
            return new List<IResponseCriterion>
            {
                new AcceptNonRetriableCriterion(),
                new RejectNetworkErrorsCriterion(),
                new RejectServerErrorsCriterion(),
                new RejectThrottlingErrorsCriterion(),
                new RejectUnknownErrorsCriterion(),
                new AlwaysAcceptCriterion()
            };
        }

        public static IReplicaOrdering ReplicaOrdering(ILog log)
        {
            var builder = new WeighedReplicaOrderingBuilder(log);

            builder.AddAdaptiveHealthModifierWithLinearDecay(AdaptiveHealthDamageDecayDuration);

            return builder.Build();
        }
    }
}
