namespace Vostok.Airlock
{
    internal interface IBufferPool
    {
        bool TryAcquire(out IBuffer buffer);

        void Release(IBuffer buffer);
    }
}