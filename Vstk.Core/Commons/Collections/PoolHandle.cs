using System;
using System.Threading;

namespace Vstk.Commons.Collections
{
    public class PoolHandle<T> : IDisposable
    {
        private IPool<T> pool;

        public PoolHandle(IPool<T> pool, T resource)
        {
            this.pool = pool;
            Resource = resource;
        }

        public T Resource { get; }

        public static implicit operator T(PoolHandle<T> handle)
        {
            return handle.Resource;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref pool, null)?.Release(Resource);
        }
    }
}
