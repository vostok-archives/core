using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport
{
    /// <summary>
    /// Represents an HTTP client used as a transport for cluster communication.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// <para>Sends given <paramref name="request"/> using provided <paramref name="timeout"/> and <paramref name="cancellationToken"/>.</para>
        /// <para>Request is guaranteed to contain an absolute url.</para>
        /// <para>This method SHOULD NOT throw exceptions. Any failures or cancellation should be expressed via <see cref="ResponseCode"/> values.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken);
    }
}
