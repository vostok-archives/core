using System;

namespace Vstk.Commons.Collections
{
    public interface IPool<T> : IDisposable
    {
        int Allocated { get; }
        int Available { get; }

        T Acquire();

        void Release(T resource);
    }
}
