using System.Collections.Generic;
using System.Linq;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities
{
    public static class ExtensionMethods
    {
        public static bool ElementwiseEquals<T>(
            this IList<T> one,
            IList<T> other)
        {
            return one.ElementwiseEquals(other, EqualityComparer<T>.Default);
        }

        public static bool ElementwiseEquals<T>(
            this IList<T> one,
            IList<T> other,
            IEqualityComparer<T> comparer)
        {
            if (ReferenceEquals(one, other))
                return true;

            if (one == null || other == null)
                return false;

            if (one.Count != other.Count)
                return false;

            for (var i = 0; i < one.Count; i++)
                if (!comparer.Equals(one[i], other[i]))
                    return false;

            return true;
        }

        public static int ElementwiseHash<T>(this IList<T> list)
        {
            return list.ElementwiseHash(EqualityComparer<T>.Default);
        }

        public static int ElementwiseHash<T>(
            this IList<T> list,
            IEqualityComparer<T> comparer)
        {
            unchecked
            {
                if (list == null)
                    return 0;

                return list.Aggregate(list.Count, (current, element) => (current * 397) ^ comparer.GetHashCode(element));
            }
        }

        //TODO: (kazakov) tests!
        public static int ElementwiseHash<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            return dic.ElementwiseHash(EqualityComparer<TValue>.Default);
        }

        public static int ElementwiseHash<TKey, TValue>(
            this IDictionary<TKey, TValue> dic,
            IEqualityComparer<TValue> valuesComparer)
        {
            unchecked
            {
                if (dic == null)
                    return 0;

                return dic.Aggregate(dic.Count, (current, keyValuePair) => (current * 397) ^ valuesComparer.GetHashCode(keyValuePair.Value));
            }
        }

        //TODO: (kazakov) tests

        public static int ElementwiseHash<TKey, TListValue>(
            this IDictionary<TKey, List<TListValue>> dic)
        {
            return dic.ElementwiseHash(EqualityComparer<TListValue>.Default);
        }

        public static int ElementwiseHash<TKey, TListValue>(
            this IDictionary<TKey, List<TListValue>> dic,
            IEqualityComparer<TListValue> comparer)
        {
            unchecked
            {
                if (dic == null)
                    return 0;

                return dic.Aggregate(dic.Count, (current, keyValuePair) => (current * 397) ^ keyValuePair.Value.ElementwiseHash(comparer));
            }
        }
    }
}