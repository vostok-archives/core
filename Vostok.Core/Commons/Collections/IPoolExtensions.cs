namespace Vostok.Commons.Collections
{
    public static class IPoolExtensions
    {
        public static PoolHandle<T> AcquireHandle<T>(this IPool<T> pool)
        {
            return new PoolHandle<T>(pool, pool.Acquire());
        }
    }
}
