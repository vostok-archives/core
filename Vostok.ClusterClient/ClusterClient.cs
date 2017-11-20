using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;
using Vostok.Clusterclient.Transport;
using Vostok.Commons.Model;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Clusterclient
{
    /// <summary>
    /// <para>Represents a client used to send HTTP requests to a cluster of replicas.</para>
    /// <para>This implementation guarantees following contracts:</para>
    /// <list type="bullet">
    /// <item>It never throws exceptions. All failures are logged and reflected in returned <see cref="ClusterResult"/> instances.</item>
    /// <item>It is thread-safe. It's recommended to reuse <see cref="ClusterClient"/> instances as much as possible.</item>
    /// <item>It sends requests with absolute urls directly and does not perform implicit resolving. You can turn them into relative ones with <see cref="IRequestTransform"/>.</item>
    /// </list>
    /// <para>A <see cref="ClusterClient"/> instance is constructed by passing an <see cref="ILog"/> and a <see cref="ClusterClientSetup"/> delegate to a constructor.</para>
    /// <para>Provided setup delegate is expected to initialize some fields of an <see cref="IClusterClientConfiguration"/> instance.</para>
    /// <para>The required minimum is to set <see cref="ITransport"/> and <see cref="IClusterProvider"/> implementations.</para>
    /// <example>
    /// <code>
    /// var client = new ClusterClient(log, config =>
    /// {
    ///     config.Transport = new MyTransport();
    ///     config.ClusterProvider = new MyClusterProvider();
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class ClusterClient : IClusterClient
    {
        private static readonly TimeSpan budgetPrecision = TimeSpan.FromMilliseconds(15);

        private readonly ClusterClientConfiguration configuration;
        private readonly Func<IRequestContext, Task<ClusterResult>> pipelineDelegate;

        /// <summary>
        /// Creates a <see cref="ClusterClient"/> instance using given <paramref name="log"/> and <paramref name="setup"/> delegate.
        /// </summary>
        /// <exception cref="ClusterClientException">Configuration was incomplete or invalid.</exception>
        public ClusterClient(ILog log, ClusterClientSetup setup)
        {
            configuration = new ClusterClientConfiguration(log ?? new SilentLog());

            setup(configuration);

            configuration.ValidateOrDie();
            configuration.AugmentWithDefaults();

            if (configuration.ReplicaTransform != null)
                configuration.ClusterProvider = new TransformingClusterProvider(configuration.ClusterProvider, configuration.ReplicaTransform);

            if (configuration.TransferDistributedContext)
                configuration.Transport = new TransportWithDistributedContext(configuration.Transport);

            if (configuration.EnableTracing)
                configuration.Transport = new TransportWithTracing(configuration.Transport);

            configuration.Transport = new TransportWithAuxiliaryHeaders(configuration.Transport, configuration);

            ReplicaStorageProvider = ReplicaStorageProviderFactory.Create(configuration.ReplicaStorageScope);

            var modules = RequestModuleChainBuilder.BuildChain(configuration, ReplicaStorageProvider);

            pipelineDelegate = RequestModuleChainBuilder.BuildChainDelegate(modules);
        }

        public IClusterProvider ClusterProvider => configuration.ClusterProvider;

        public IReplicaStorageProvider ReplicaStorageProvider { get; }

        public Task<ClusterResult> SendAsync(
            Request request,
            TimeSpan? timeout = null,
            IRequestStrategy strategy = null,
            CancellationToken cancellationToken = default(CancellationToken),
            RequestPriority? priority = null)
        {
            return pipelineDelegate(
                CreateContext(
                    request,
                    timeout ?? configuration.DefaultTimeout,
                    strategy ?? configuration.DefaultRequestStrategy,
                    cancellationToken,
                    priority ?? configuration.DefaultPriority,
                    configuration.MaxReplicasUsedPerRequest)
            );
        }

        private RequestContext CreateContext(Request request, TimeSpan timeout, IRequestStrategy strategy, CancellationToken cancellationToken, RequestPriority? priority, int maxReplicasToUse)
        {
            return new RequestContext(request, strategy, RequestTimeBudget.StartNew(timeout, budgetPrecision), configuration.Log, cancellationToken, priority, maxReplicasToUse);
        }
    }
}
