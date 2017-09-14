using System;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Synchronization
{
    public interface IAsyncLock
    {
        Awaitable<IDisposable> LockAsync();
    }
}