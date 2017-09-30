using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Airlock;

namespace Vostok.AirlockClient
{
    public interface IAirlockClient
    {
        Task<AirlockResponse> PingAsync(TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken));

        Task<AirlockResponse> SendAsync(AirlockMessage message, TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}