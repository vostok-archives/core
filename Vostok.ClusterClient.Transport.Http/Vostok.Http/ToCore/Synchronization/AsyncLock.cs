using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Synchronization
{
    public class AsyncLock : IAsyncLock
    {
        public AsyncLock()
        {
            semaphore = new SemaphoreSlim(1, 1);
            cachedReleaserTask = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Awaitable<IDisposable> LockAsync()
        {
            Task waitTask = semaphore.WaitAsync();
            if (waitTask.IsCompleted)
                return new Awaitable<IDisposable>(cachedReleaserTask);

            return new Awaitable<IDisposable>(waitTask.ContinueWith((_, state) => (IDisposable)new Releaser((AsyncLock)state), this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));
        }

        public Boolean TryLockImmediately(out IDisposable releaser)
        {
            if (semaphore.CurrentCount == 1 && semaphore.WaitAsync(0).GetAwaiter().GetResult())
            {
                releaser = cachedReleaserTask.Result;
                return true;
            }

            releaser = null;
            return false;
        }

        private readonly SemaphoreSlim semaphore;
        private readonly Task<IDisposable> cachedReleaserTask;

        #region Releaser

        private class Releaser : IDisposable
        {
            public Releaser(AsyncLock lockToRelease)
            {
                this.lockToRelease = lockToRelease;
            }

            public void Dispose()
            {
                lockToRelease.semaphore.Release();
            }

            private readonly AsyncLock lockToRelease;
        }

        #endregion
    }
}