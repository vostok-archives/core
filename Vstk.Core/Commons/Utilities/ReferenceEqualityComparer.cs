using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vstk.Commons.Utilities
{
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}