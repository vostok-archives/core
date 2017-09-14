namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Collections
{
    internal interface IPoolStorage<T>
    {
        bool TryAcquire(out T resource);

        void Put(T resource);

        int Count { get; }
    }
}