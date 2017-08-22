using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Vostok.Commons.Collections
{
    public class UnlimitedLazyPool<T> : IPool<T>
    {
        private readonly Func<T> resourceFactory;
        private readonly ConcurrentQueue<T> resourceStorage;
        private int allocated;

        public UnlimitedLazyPool(Func<T> resourceFactory)
        {
            this.resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
            resourceStorage = new ConcurrentQueue<T>();
        }

        public int Allocated => allocated;

        public int Available => resourceStorage.Count;

        public T Acquire()
        {
            if (!resourceStorage.TryDequeue(out var resource))
            {
                resource = resourceFactory();
                Interlocked.Increment(ref allocated);
            }

            return resource;
        }

        public void Release(T resource)
        {
            resourceStorage.Enqueue(resource);
        }
    }
}
