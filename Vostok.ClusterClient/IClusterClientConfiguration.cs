using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Ordering;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed;
using Vostok.Clusterclient.Retry;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;
using Vostok.Clusterclient.Transport;
using Vostok.Commons.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient
{
    /// <summary>
    /// <para>Represents a configuration of <see cref="ClusterClient"/> instance which must be filled during client construction.</para>
    /// <para>The only required parameters are <see cref="Transport"/> and <see cref="ClusterProvider"/>.</para>
    /// </summary>
    public interface IClusterClientConfiguration
    {
        /// <summary>
        /// Returns an <see cref="ILog"/> instance which can be used to construct other parts of configuration.
        /// </summary>
        [NotNull]
        ILog Log { get; }

        /// <summary>
        /// <para>A transport (HTTP client) implementation used to send requests. See <see cref="ITransport"/> for more details.</para>
        /// <para>This parameter is REQUIRED.</para>
        /// </summary>
        ITransport Transport { get; set; }

        /// <summary>
        /// <para>An implementation of cluster provider. See <see cref="IClusterProvider.GetCluster"/> for more details.</para>
        /// <para>This parameter is REQUIRED.</para>
        /// </summary>
        IClusterProvider ClusterProvider { get; set; }

        /// <summary>
        /// <para>Gets or sets replica addresses transform. See <see cref="IReplicaTransform"/> for more details.</para>
        /// <para>This parameter is optional.</para>
        /// </summary>
        IReplicaTransform ReplicaTransform { get; set; }

        /// <summary>
        /// <para>Gets or sets replica ordering implementation. See <see cref="IReplicaOrdering.Order"/> and <see cref="IReplicaOrdering.Learn"/> for more details.</para>
        /// <para>The recommended implementation is <see cref="WeighedReplicaOrdering"/>. Use <see cref="ClusterClientConfigurationExtensions.SetupWeighedReplicaOrdering"/> extension to build it.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.ReplicaOrdering"/>).</para>
        /// </summary>
        IReplicaOrdering ReplicaOrdering { get; set; }

        /// <summary>
        /// <para>Gets or sets the replica storage scope. See <see cref="ReplicaStorageScope"/> and <see cref="IReplicaStorageProvider"/> for more details.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.ReplicaStorageScope"/>).</para>
        /// </summary>
        ReplicaStorageScope ReplicaStorageScope { get; set; }

        /// <summary>
        /// <para>A list of request transforms. See <see cref="IRequestTransform"/> for more details.</para>
        /// <para>Use <see cref="ClusterClientConfigurationExtensions.AddRequestTransform(IClusterClientConfiguration, IRequestTransform)"/> to add transforms to this list.</para>
        /// <para>This parameter is optional and has an empty default value.</para>
        /// </summary>
        List<IRequestTransform> RequestTransforms { get; set; }

        /// <summary>
        /// <para>A list of response transforms. See <see cref="IResponseTransform"/> for more details.</para>
        /// <para>Use <see cref="ClusterClientConfigurationExtensions.AddResponseTransform(IClusterClientConfiguration, IResponseTransform)"/> to add transforms to this list.</para>
        /// <para>This parameter is optional and has an empty default value.</para>
        /// </summary>
        List<IResponseTransform> ResponseTransforms { get; set; }

        /// <summary>
        /// <para>A list of response criteria. See <see cref="IResponseCriterion"/> and <see cref="ResponseVerdict"/> for more details.</para>
        /// <para>Use <see cref="ClusterClientConfigurationExtensions.SetupResponseCriteria"/> to initialize this list.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.ResponseCriteria"/>).</para>
        /// </summary>
        List<IResponseCriterion> ResponseCriteria { get; set; }

        /// <summary>
        /// <para>A list of additional user-defined request modules. These modules are inserted into native execution pipeline.</para>
        /// <para>See <see cref="IRequestModule"/> interface for more details about request modules.</para>
        /// <para>Final execution pipeline looks like this:</para>
        /// <list type="number">
        /// <item>Exception logging and handling.</item>
        /// <item>Request transformation (application of <see cref="IRequestTransform"/> chain).</item>
        /// <item>Request priority application (adding a priority header to request).</item>
        /// <item>User-defined <see cref="IRequestModule"/> implementations.</item>
        /// <item>Request/result logging.</item>
        /// <item>Exception logging and handling.</item>
        /// <item>Request validation.</item>
        /// <item>Timeout validation.</item>
        /// <item>Retry loop (application of <see cref="IRetryPolicy"/> and <see cref="IRetryStrategy"/>).</item>
        /// <item>Sending of requests with absolute urls (directly using <see cref="ITransport"/>).</item>
        /// <item>Request execution (<see cref="IClusterProvider"/> --> <see cref="IReplicaOrdering"/> --> <see cref="IRequestStrategy"/>)</item>
        /// </list>
        /// <para>Use <see cref="ClusterClientConfigurationExtensions.AddRequestModule"/> to add transforms to this list.</para>
        /// <para>This parameter is optional and has an empty default value.</para>
        /// </summary>
        List<IRequestModule> Modules { get; set; }

        /// <summary>
        /// <para>Gets or sets retry policy. See <see cref="IRetryPolicy"/> for more details.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.RetryPolicy"/>).</para>
        /// </summary>
        IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// <para>Gets or sets retry strategy. See <see cref="IRetryStrategy"/> for more details.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.RetryStrategy"/>).</para>
        /// </summary>
        IRetryStrategy RetryStrategy { get; set; }

        /// <summary>
        /// <para>Gets or sets the response selector. See <see cref="IResponseSelector.Select"/> for more details.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.ResponseSelector"/>).</para>
        /// </summary>
        IResponseSelector ResponseSelector { get; set; }

        /// <summary>
        /// <para>Gets or sets a default request strategy used for <see cref="ClusterClient"/> method overloads without strategy parameter.</para>
        /// <para>See <see cref="IRequestStrategy.SendAsync"/> for more details about what a request strategy is.</para>
        /// <para>See <see cref="Strategy"/> class for some prebuilt strategies and convenient factory methods.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.RequestStrategy"/>).</para>
        /// </summary>
        IRequestStrategy DefaultRequestStrategy { get; set; }

        /// <summary>
        /// <para>Gets or sets a default request timeout used for <see cref="ClusterClient"/> method overloads without timeout parameter.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.Timeout"/>).</para>
        /// </summary>
        TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// <para>Gets or sets a default request priority used for <see cref="ClusterClient"/> method overloads without priority parameter.</para>
        /// <para>This parameter is optional and has a <c>null</c> default value.</para>
        /// </summary>
        RequestPriority? DefaultPriority { get; set; }

        /// <summary>
        /// <para>Gets or sets a limit on how many replicas a single request may use. Such a limit is useful to contain uncontrollable retry explosions.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.MaxReplicasUsedPerRequest"/>).</para>
        /// </summary>
        int MaxReplicasUsedPerRequest { get; set; }

        /// <summary>
        /// <para>Gets or sets the options for adaptive throttling mechanism (described in https://landing.google.com/sre/book/chapters/handling-overload.html ).</para>
        /// <para>This parameter is optional and has a <c>null</c> default value which implies no such throttling will be used.</para>
        /// <para>Use <see cref="ClusterClientConfigurationExtensions.SetupAdaptiveThrottling"/> to set these options.</para>
        /// </summary>
        AdaptiveThrottlingOptions AdaptiveThrottling { get; set; }

        /// <summary>
        /// <para>Gets or sets the options for replica budgeting mechanism which attempts to limit the ratio of used replicas to issued requests.</para>
        /// <para>This parameter is optional and has a <c>null</c> default value which implies no such budgeting will be used.</para>
        /// <para>Use <see cref="ClusterClientConfigurationExtensions.SetupReplicaBudgeting"/> to set these options.</para>
        /// </summary>
        ReplicaBudgetingOptions ReplicaBudgeting { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to log request details before execution.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.LogRequestDetails"/>).</para>
        /// </summary>
        bool LogRequestDetails { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to log result details after execution.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.LogResultDetails"/>).</para>
        /// </summary>
        bool LogResultDetails { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to log requests to each replica.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.LogReplicaRequests"/>).</para>
        /// </summary>
        bool LogReplicaRequests { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to log results from each replica.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.LogReplicaResults"/>).</para>
        /// </summary>
        bool LogReplicaResults { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to transfer distributed context in request.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.TransferDistributedContext"/>).</para>
        /// </summary>
        bool TransferDistributedContext { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to include a <see cref="HeaderNames.XKonturRequestTimeout"/> header in request.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.IncludeRequestTimeoutHeader"/>).</para>
        /// </summary>
        bool IncludeRequestTimeoutHeader { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to include a <see cref="HeaderNames.XKonturClientIdentity"/> header in request.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.IncludeClientIdentityHeader"/>).</para>
        /// </summary>
        bool IncludeClientIdentityHeader { get; set; }

        /// <summary>
        /// <para>Gets or sets whether to enable tracing.</para>
        /// <para>This parameter is optional and has a default value (see <see cref="ClusterClientDefaults.EnableTracing"/>).</para>
        /// </summary>
        bool EnableTracing { get; set; }

        /// <summary>
        /// <para>Gets or sets service name.</para>
        /// <para>This parameter is optional and has no default value.</para>
        /// </summary>
        string ServiceName { get; set; }
    }
}
