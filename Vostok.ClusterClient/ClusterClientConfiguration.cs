using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Ordering;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Retry;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;
using Vostok.Clusterclient.Transport;
using Vostok.Commons.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient
{
    internal class ClusterClientConfiguration : IClusterClientConfiguration
    {
        public ClusterClientConfiguration(ILog log)
        {
            Log = log;
            RequestTransforms = new List<IRequestTransform>();
            ResponseTransforms = new List<IResponseTransform>();
            ResponseCriteria = new List<IResponseCriterion>();
            Modules = new List<IRequestModule>();
            ReplicaStorageScope = ClusterClientDefaults.ReplicaStorageScope;
            DefaultTimeout = ClusterClientDefaults.Timeout;
            LogRequestDetails = ClusterClientDefaults.LogRequestDetails;
            LogResultDetails = ClusterClientDefaults.LogResultDetails;
            LogReplicaRequests = ClusterClientDefaults.LogReplicaRequests;
            LogReplicaResults = ClusterClientDefaults.LogReplicaResults;
            MaxReplicasUsedPerRequest = ClusterClientDefaults.MaxReplicasUsedPerRequest;
            TransferDistributedContext = ClusterClientDefaults.TransferDistributedContext;
            IncludeRequestTimeoutHeader = ClusterClientDefaults.IncludeRequestTimeoutHeader;
            IncludeClientIdentityHeader = ClusterClientDefaults.IncludeClientIdentityHeader;
            EnableTracing = ClusterClientDefaults.EnableTracing;
        }

        public ILog Log { get; }

        public ITransport Transport { get; set; }

        public IClusterProvider ClusterProvider { get; set; }

        public IReplicaTransform ReplicaTransform { get; set; }

        public IReplicaOrdering ReplicaOrdering { get; set; }

        public ReplicaStorageScope ReplicaStorageScope { get; set; }

        public List<IRequestTransform> RequestTransforms { get; set; }

        public List<IResponseTransform> ResponseTransforms { get; set; }

        public List<IResponseCriterion> ResponseCriteria { get; set; }

        public List<IRequestModule> Modules { get; set; }

        public IRetryPolicy RetryPolicy { get; set; }

        public IRetryStrategy RetryStrategy { get; set; }

        public IResponseSelector ResponseSelector { get; set; }

        public IRequestStrategy DefaultRequestStrategy { get; set; }

        public TimeSpan DefaultTimeout { get; set; }

        public RequestPriority? DefaultPriority { get; set; }

        public int MaxReplicasUsedPerRequest { get; set; }

        public AdaptiveThrottlingOptions AdaptiveThrottling { get; set; }

        public ReplicaBudgetingOptions ReplicaBudgeting { get; set; }

        public bool LogRequestDetails { get; set; }

        public bool LogResultDetails { get; set; }

        public bool LogReplicaRequests { get; set; }

        public bool LogReplicaResults { get; set; }

        public bool TransferDistributedContext { get; set; }

        public bool IncludeRequestTimeoutHeader { get; set; }

        public bool IncludeClientIdentityHeader { get; set; }

        public bool EnableTracing { get; set; }

        public string ServiceName { get; set; }

        public bool IsValid => !Validate().Any();

        public IEnumerable<string> Validate()
        {
            if (Transport == null)
                yield return "Transport implementation is not set. It is a required part of configuration.";

            if (ClusterProvider == null)
                yield return "Cluster provider implementation is not set. It is a required part of configuration.";

            if (ResponseCriteria?.Count > 0)
            {
                var lastCriterion = ResponseCriteria.Last();
                if (!(lastCriterion is AlwaysRejectCriterion) && !(lastCriterion is AlwaysAcceptCriterion))
                    yield return $"Last response criterion must always be either an '{typeof (AlwaysRejectCriterion).Name}' or '{typeof (AlwaysAcceptCriterion).Name}'.";
            }

            if (ResponseCriteria != null && ResponseCriteria.Any(criterion => criterion == null))
                yield return "One of provided response criteria is null";

            if (RequestTransforms != null && RequestTransforms.Any(transform => transform == null))
                yield return "One of provided request transforms is null";

            if (ResponseTransforms != null && ResponseTransforms.Any(transform => transform == null))
                yield return "One of provided response transforms is null";

            if (Modules != null && Modules.Any(module => module == null))
                yield return "One of provided request modules is null";

            if (DefaultTimeout <= TimeSpan.Zero)
                yield return $"Default timeout must be positive, but was '{DefaultTimeout}'";

            if (MaxReplicasUsedPerRequest <= 0)
                yield return $"Maximum replicas per request must be > 0, but was {MaxReplicasUsedPerRequest}";
        }

        public void ValidateOrDie()
        {
            if (IsValid)
                return;

            var builder = new StringBuilder();

            builder.AppendLine("There are some errors in cluster client configuration:");

            foreach (var errorMessage in Validate())
            {
                builder.Append("\t");
                builder.Append("--> ");
                builder.AppendLine(errorMessage);
            }

            throw new ClusterClientException(builder.ToString());
        }

        public void AugmentWithDefaults()
        {
            if (ReplicaOrdering == null)
                ReplicaOrdering = ClusterClientDefaults.ReplicaOrdering(Log);

            if (ResponseCriteria == null || ResponseCriteria.Count == 0)
                ResponseCriteria = ClusterClientDefaults.ResponseCriteria();

            if (RetryPolicy == null)
                RetryPolicy = ClusterClientDefaults.RetryPolicy;

            if (RetryStrategy == null)
                RetryStrategy = ClusterClientDefaults.RetryStrategy;

            if (ResponseSelector == null)
                ResponseSelector = ClusterClientDefaults.ResponseSelector;

            if (DefaultRequestStrategy == null)
                DefaultRequestStrategy = ClusterClientDefaults.RequestStrategy;
        }
    }
}
