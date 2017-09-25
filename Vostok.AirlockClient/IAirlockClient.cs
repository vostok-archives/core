using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Airlock;

namespace Vostok.AirlockClient
{
    public interface IAirlockClient
    {
        // TODO: Special type for response instead of bool
        Task<bool> PingAsync(TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken));

        // TODO: Special type for response instead of bool
        Task<bool> SendAsync(AirlockMessage message, TimeSpan timeout = default(TimeSpan),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}