using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.Utilities
{
    public static class ExtensionMethods
    {
        public static string DoFormat(this string input, params object[] parameters)
        {
            return string.Format(input, parameters);
        }

        public static string[] SplitRemovingEmpties(this string input, params char[] separator)
        {
            return input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitRemovingEmpties(this string input, params string[] separator)
        {
            return input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ReverseString(this string input)
        {
            var charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static bool ElementwiseEquals<T>(
            this T[] one,
            T[] other)
        {
            return one.ElementwiseEquals(other, EqualityComparer<T>.Default);
        }

        public static bool ElementwiseEquals<T>(
            this T[] one,
            T[] other,
            IEqualityComparer<T> comparer)
        {
            if (ReferenceEquals(one, other))
                return true;
            if (one == null || other == null)
                return false;
            if (one.Length != other.Length)
                return false;

            for (var i = 0; i < one.Length; i++)
            {
                if (!comparer.Equals(one[i], other[i]))
                    return false;
            }
            return true;
        }

        public static bool ElementwiseEquals<T>(
            this T[] one,
            T[] other,
            int otherOffset,
            int otherLen)
        {
            return one.ElementwiseEquals(other, otherOffset, otherLen, EqualityComparer<T>.Default);
        }

        public static bool ElementwiseEquals<T>(
            this T[] one,
            T[] other,
            int otherOffset,
            int otherLen,
            IEqualityComparer<T> comparer)
        {
            if (ReferenceEquals(null, other))
                return ReferenceEquals(null, one);
            if (ReferenceEquals(null, one))
                return false;
            if (one.Length != otherLen)
                return false;

            for (var i = 0; i < one.Length; i++)
            {
                if (!comparer.Equals(one[i], other[i + otherOffset]))
                    return false;
            }
            return true;
        }

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

        public static bool ElementwiseEquals<TKey, TValue>(
            this IDictionary<TKey, TValue> one,
            IDictionary<TKey, TValue> other)
        {
            return one.ElementwiseEquals(other, EqualityComparer<TValue>.Default);
        }

        public static bool ElementwiseEquals<TKey, TValue>(
            this IDictionary<TKey, TValue> one,
            IDictionary<TKey, TValue> other,
            IEqualityComparer<TValue> valuesComparer)
        {
            if (ReferenceEquals(null, other))
                return ReferenceEquals(null, one);
            if (ReferenceEquals(null, one))
                return false;
            if (ReferenceEquals(one, other))
                return true;

            if (one.Count != other.Count)
                return false;

            foreach (var pair in one)
            {
                if (!other.ContainsKey(pair.Key))
                    return false;
                if (!valuesComparer.Equals(pair.Value, other[pair.Key]))
                    return false;
            }

            return true;
        }

        public static bool ElementwiseEquals<TKey, TKey2, TValue2>(
            this IDictionary<TKey, Dictionary<TKey2, TValue2>> one,
            IDictionary<TKey, Dictionary<TKey2, TValue2>> other)
        {
            return one.ElementwiseEquals(other, EqualityComparer<TValue2>.Default);
        }

        public static bool ElementwiseEquals<TKey, TKey2, TValue2>(
            this IDictionary<TKey, Dictionary<TKey2, TValue2>> one,
            IDictionary<TKey, Dictionary<TKey2, TValue2>> other,
            IEqualityComparer<TValue2> valuesComparer)
        {
            if (ReferenceEquals(null, other))
                return ReferenceEquals(null, one);
            if (ReferenceEquals(null, one))
                return false;
            if (ReferenceEquals(one, other))
                return true;

            if (one.Count != other.Count)
                return false;

            foreach (var pair in one)
            {
                if (!other.ContainsKey(pair.Key))
                    return false;
                if (!pair.Value.ElementwiseEquals(other[pair.Key], valuesComparer))
                    return false;
            }

            return true;
        }

        public static bool ElementwiseEquals<TKey, TListValue>(
            this IDictionary<TKey, List<TListValue>> one,
            IDictionary<TKey, List<TListValue>> other)
        {
            return one.ElementwiseEquals(other, EqualityComparer<TListValue>.Default);
        }

        public static bool ElementwiseEquals<TKey, TListValue>(
            this IDictionary<TKey, List<TListValue>> one,
            IDictionary<TKey, List<TListValue>> other,
            IEqualityComparer<TListValue> comparer)
        {
            if (ReferenceEquals(null, other))
                return ReferenceEquals(null, one);
            if (ReferenceEquals(null, one))
                return false;
            if (ReferenceEquals(one, other))
                return true;

            if (one.Count != other.Count)
                return false;

            foreach (var pair in one)
            {
                if (!other.ContainsKey(pair.Key))
                    return false;
                if (!pair.Value.ElementwiseEquals(other[pair.Key], comparer))
                    return false;
            }
            return true;
        }

        public static bool ElementwiseEquals<TKey, TListValue>(
            this IDictionary<TKey, TListValue[]> one,
            IDictionary<TKey, TListValue[]> other)
        {
            return one.ElementwiseEquals(other, EqualityComparer<TListValue>.Default);
        }

        public static bool ElementwiseEquals<TKey, TListValue>(
            this IDictionary<TKey, TListValue[]> one,
            IDictionary<TKey, TListValue[]> other,
            IEqualityComparer<TListValue> comparer)
        {
            if (ReferenceEquals(null, other))
                return ReferenceEquals(null, one);
            if (ReferenceEquals(null, one))
                return false;
            if (ReferenceEquals(one, other))
                return true;

            if (one.Count != other.Count)
                return false;

            foreach (var pair in one)
            {
                if (!other.ContainsKey(pair.Key))
                    return false;
                if (!pair.Value.ElementwiseEquals(other[pair.Key], comparer))
                    return false;
            }
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

        public static int ElementwiseHash<T>(this ICollection<T> collection)
        {
            return collection.ElementwiseHash(EqualityComparer<T>.Default);
        }

        public static int ElementwiseHash<T>(
            this ICollection<T> collection,
            IEqualityComparer<T> comparer)
        {
            unchecked
            {
                if (collection == null)
                    return 0;

                return collection.Aggregate(collection.Count, (current, element) => (current * 397) ^ comparer.GetHashCode(element));
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

        public static int ElementwiseHash<TKey, TListValue>(
            this IDictionary<TKey, TListValue[]> dic)
        {
            return dic.ElementwiseHash(EqualityComparer<TListValue>.Default);
        }

        public static int ElementwiseHash<TKey, TListValue>(
            this IDictionary<TKey, TListValue[]> dic,
            IEqualityComparer<TListValue> comparer)
        {
            unchecked
            {
                if (dic == null)
                    return 0;

                return dic.Aggregate(dic.Count, (current, keyValuePair) => (current * 397) ^ keyValuePair.Value.ElementwiseHash(comparer));
            }
        }

        public static StringBuilder AppendFormatLine(this StringBuilder stringBuilder, string format, params object[] parameters)
        {
            return stringBuilder.AppendLine(string.Format(format, parameters));
        }

        public static void RemoveLastChar(this StringBuilder stringBuilder)
        {
            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
                action(t);
        }

        public static void ForEach<T>(this IList<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
                action(t);
        }

        public static string ToOneString<T>(this IEnumerable<T> objects, string devider)
        {
            return ToOneString(objects, devider, o => o.ToString());
        }

        public static string ToOneString<T>(this IEnumerable<T> objects, string devider, Func<T, string> toString)
        {
            var res = new StringBuilder();
            objects.ForEach(
                obj =>
                {
                    res.Append(toString(obj));
                    res.Append(devider);
                });
            return res.Length > 0 ? res.ToString(0, res.Length - devider.Length) : "";
        }

        public static void AddDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            dic.Add(key, default(TValue));
        }

        public static T NotEmpty<T>(this T c, Action onFailure)
            where T : class, ICollection
        {
            if (c == null || c.Count == 0)
            {
                if (onFailure != null)
                    onFailure();
            }
            return c;
        }

        public static T Or<T>(this T c, T def)
            where T : class, ICollection
        {
            if (c == null || c.Count == 0)
                return def;
            return c;
        }

        public static U At<T, U>(this T c, int index, U def)
            where T : class, ICollection<U>, IList<U>
        {
            if (c == null || c.Count <= index)
                return def;
            return c[index];
        }

        public static TValue GetValueOrDie<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            if (dic.TryGetValue(key, out value))
                return value;
            throw new KeyNotFoundException(key.ToString());
        }

        public static Stream SeekBegin(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static int GetHashCode<T>(this T value, IEqualityComparer<T> comparer) => comparer.GetHashCode(value);
    }
}