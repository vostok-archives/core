using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Airlock;

namespace Vostok.AirlockClient
{
    public interface IAirlockClient
    {
        Task<PingResponse> PingAsync(TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken));

        Task<SendResponse> SendAsync(AirlockMessage message, TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}