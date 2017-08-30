using System;

namespace Vostok.Clusterclient.Helpers
{
    internal struct PoolHandle<T> : IDisposable
        where T : class
    {
        private readonly Pool<T> pool;
        private readonly T resource;

        public PoolHandle(Pool<T> pool, T resource)
        {
            this.pool = pool;
            this.resource = resource;
        }

        public T Resource => resource;

        public void Dispose()
        {
            pool.Release(resource);
        }

        public static implicit operator T(PoolHandle<T> handle)
        {
            return handle.resource;
        }
    }
}
