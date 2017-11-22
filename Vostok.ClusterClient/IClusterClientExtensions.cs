using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies;
using Vostok.Commons.Model;
using Vostok.Flow;
using Vostok.Tracing;

namespace Vostok.Clusterclient
{
    public static class ClusterClientExtensions
    {
        /// <summary>
        /// <para>Sends given request using given <paramref name="timeout"/>, <paramref name="strategy"/> and <paramref name="cancellationToken"/>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultTimeout"/> if provided <paramref name="timeout"/> is <c>null</c>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultRequestStrategy"/> if provided <paramref name="strategy"/> is <c>null</c>.</para>
        /// <para>See <see cref="IRequestStrategy.SendAsync"/> for more details about what a request strategy is.</para>
        /// <para>See <see cref="Strategy"/> class for some prebuilt strategies and convenient factory methods.</para>
        /// </summary>
        [NotNull]
        public static async Task<ClusterResult> SendAsync(
            [NotNull] this IClusterClient client,
            [NotNull] Request request,
            [CanBeNull] TimeSpan? timeout = null,
            [CanBeNull] IRequestStrategy strategy = null,
            CancellationToken cancellationToken = default(CancellationToken),
            [CanBeNull] RequestPriority? priority = null,
            [CanBeNull] string operationName = null)
        {
            using (operationName == null ? null : Context.Properties.Use(TracingAnnotationNames.Operation, operationName))
            {
                return await client.SendAsync(request, timeout, strategy, cancellationToken, priority).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// <para>Sends given request using given <paramref name="timeout"/>, <paramref name="strategy"/> and <paramref name="cancellationToken"/>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultTimeout"/> if provided <paramref name="timeout"/> is <c>null</c>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultRequestStrategy"/> if provided <paramref name="strategy"/> is <c>null</c>.</para>
        /// <para>See <see cref="IRequestStrategy.SendAsync"/> for more details about what a request strategy is.</para>
        /// <para>See <see cref="Strategy"/> class for some prebuilt strategies and convenient factory methods.</para>
        /// </summary>
        [NotNull]
        public static ClusterResult Send(
            [NotNull] this IClusterClient client,
            [NotNull] Request request,
            [CanBeNull] TimeSpan? timeout = null,
            [CanBeNull] IRequestStrategy strategy = null,
            CancellationToken cancellationToken = default(CancellationToken),
            [CanBeNull] RequestPriority? priority = null,
            [CanBeNull] string operationName = null)
        {
            using (operationName == null ? null : Context.Properties.Use(TracingAnnotationNames.Operation, operationName))
            {
                return client.SendAsync(request, timeout, strategy, cancellationToken, priority).GetAwaiter().GetResult();
            }
        }
    }
}
