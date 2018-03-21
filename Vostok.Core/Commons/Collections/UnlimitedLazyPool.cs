using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Vostok.Commons.Collections
{
    public class UnlimitedLazyPool<T> : IPool<T>
    {
        private readonly Func<T> resourceFactory;
        private readonly ConcurrentQueue<T> resourceStorage;
        private readonly bool resourceIsDisposable;
        private int allocated;
        private bool isDisposed;

        public UnlimitedLazyPool(Func<T> resourceFactory)
        {
            this.resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
            resourceStorage = new ConcurrentQueue<T>();
            resourceIsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(T));
            isDisposed = false;
        }

        public int Allocated => allocated;

        public int Available => resourceStorage.Count;

        public T Acquire()
        {
            if (isDisposed)
                throw new ObjectDisposedException("Pool is closed");

            if (resourceStorage.TryDequeue(out var resource))
                return resource;
            
            resource = resourceFactory();
            Interlocked.Increment(ref allocated);
            return resource;
        }

        public void Release(T resource)
        {
            if (isDisposed)
            {
                DisposeResource(resource);
            }
            else
            {
                resourceStorage.Enqueue(resource);
                if (isDisposed)
                {
                    TryDisposeResourceFromStorage();
                }
            }
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            while (TryDisposeResourceFromStorage())
            { }
        }

        private bool TryDisposeResourceFromStorage()
        {
            if (!resourceStorage.TryDequeue(out var resource))
            {
                return false;
            }
            
            Interlocked.Decrement(ref allocated);
            DisposeResource(resource);
            return true;
        }

        private void DisposeResource(T resource)
        {
            if (resourceIsDisposable)
            {
                ((IDisposable) resource).Dispose();
            }
        }
    }
}
