using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;

namespace Vostok.Clusterclient.Strategies
{
    public interface IRequestStrategy
    {
        /// <summary>
        /// <para>Sends given request to one or more replicas until a satisfactory result is obtained or replicas are exhausted.</para>
        /// <para>Implementations are expected to obey following rules:</para>
        /// <list type="bullet">
        /// <item>This method MUST return when it receives a good enough result. Typically, this is any result with <see cref="ResponseVerdict.Accept"/> verdict.</item>
        /// <item>This method MUST return when it uses all replicas.</item>
        /// <item>This method MUST be thread-safe.</item>
        /// <item>This method MUST NOT return any data, all replica results are stored automatically.</item>
        /// <item>This method MUST NOT buffer or reorder replicas sequence.</item>
        /// <item>This method MUST NOT reuse already contacted replicas.</item>
        /// <item>This method MUST NOT exceed time budget. Use <see cref="IRequestTimeBudget.Remaining"/> to check how much time's left.</item>
        /// </list>
        /// </summary>
        /// <param name="request">A request that needs to be sent.</param>
        /// <param name="sender">A tool to send request to replicas.</param>
        /// <param name="budget">Request time budget.</param>
        /// <param name="replicas">Ordered replicas sequence.</param>
        /// <param name="replicasCount">Total replicas count.</param>
        /// <param name="cancellationToken">A cancellation token used for request execution.</param>
        Task SendAsync(
            [NotNull] Request request,
            [NotNull] IRequestSender sender,
            [NotNull] IRequestTimeBudget budget,
            [NotNull] IEnumerable<Uri> replicas,
            int replicasCount,
            CancellationToken cancellationToken);
    }
}
