namespace Vostok.Commons.Collections
{
    public interface IPool<T>
    {
        int Allocated { get; }
        int Available { get; }

        T Acquire();

        void Release(T resource);
    }
}
