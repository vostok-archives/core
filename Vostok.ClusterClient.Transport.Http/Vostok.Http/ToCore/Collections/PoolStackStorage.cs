using System.Collections.Concurrent;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Collections
{
    internal class PoolStackStorage<T> : IPoolStorage<T>
    {
        public PoolStackStorage()
        {
            stack = new ConcurrentStack<T>();
        }

        public bool TryAcquire(out T resource)
        {
            return stack.TryPop(out resource);
        }

        public void Put(T resource)
        {
            stack.Push(resource);
        }

        public int Count => stack.Count;

        private readonly ConcurrentStack<T> stack;
    }
}