using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies;
using Vostok.Commons.Model;

namespace Vostok.Clusterclient
{
    public interface IClusterClient
    {
        /// <summary>
        /// <para>Sends given request using given <paramref name="timeout"/>, <paramref name="strategy"/>, <paramref name="cancellationToken"/> and <paramref name="priority"/>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultTimeout"/> if provided <paramref name="timeout"/> is <c>null</c>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultRequestStrategy"/> if provided <paramref name="strategy"/> is <c>null</c>.</para>
        /// <para>Uses <see cref="IClusterClientConfiguration.DefaultPriority"/> or priority from current ambient context if provided <paramref name="priority"/> is <c>null</c> (explicit > default > context).</para>
        /// <para>See <see cref="IRequestStrategy.SendAsync"/> for more details about what a request strategy is.</para>
        /// <para>See <see cref="Strategy"/> class for some prebuilt strategies and convenient factory methods.</para>
        /// </summary>
        [ItemNotNull]
        Task<ClusterResult> SendAsync(
            [NotNull] Request request, 
            [CanBeNull] TimeSpan? timeout = null, 
            [CanBeNull] IRequestStrategy strategy = null, 
            CancellationToken cancellationToken = default(CancellationToken),
            [CanBeNull] RequestPriority? priority = null);
    }
}
