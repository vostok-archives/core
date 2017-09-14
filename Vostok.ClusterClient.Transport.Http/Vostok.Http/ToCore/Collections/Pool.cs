using System;
using System.Threading;
using Vostok.Commons.Collections;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Collections
{
    public class Pool<T> : IPool<T>
       where T : class
    {
        public Pool(Func<T> resourceFactory, PoolAccessStrategy accessStrategy = PoolAccessStrategy.FIFO)
        {
            this.resourceFactory = resourceFactory;

            resourceStorage = CreateStorage(accessStrategy);
        }

        public T Acquire()
        {
            CheckNotDisposed();

            if (!resourceStorage.TryAcquire(out var resource))
            {
                resource = resourceFactory();
                Interlocked.Increment(ref allocated);
            }

            return resource;
        }

        public void Release(T resource)
        {
            if (disposed)
            {
                TryDispose(resource);
            }
            else
            {
                resourceStorage.Put(resource);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                while (resourceStorage.TryAcquire(out var resource))
                    TryDispose(resource);
            }
        }

        public int Allocated => allocated;

        public int Available => resourceStorage.Count;

        private void CheckNotDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("pool", "Can't operate on disposed pool.");
        }

        private static void TryDispose(T resource)
        {
            if (resource is IDisposable disposable)
                disposable.Dispose();
        }

        private static IPoolStorage<T> CreateStorage(PoolAccessStrategy strategy)
        {
            switch (strategy)
            {
                case PoolAccessStrategy.FIFO:
                    return new PoolQueueStorage<T>();

                case PoolAccessStrategy.LIFO:
                    return new PoolStackStorage<T>();

                default:
                    throw new ArgumentOutOfRangeException("strategy", "Unexpected access strategy: " + strategy);
            }
        }

        private readonly Func<T> resourceFactory;
        private readonly IPoolStorage<T> resourceStorage;

        private int allocated;
        private volatile bool disposed;
    }
}