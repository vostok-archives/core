namespace Vstk.Commons.Collections
{
    public static class PoolExtensions
    {
        public static PoolHandle<T> AcquireHandle<T>(this IPool<T> pool)
        {
            return new PoolHandle<T>(pool, pool.Acquire());
        }
    }
}
