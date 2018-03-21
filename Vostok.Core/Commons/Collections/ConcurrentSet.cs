using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vostok.Commons.Collections
{
    public class ConcurrentSet<T> : ISet<T>
    {
        private readonly ConcurrentDictionary<T, byte> backingDictionary;

        public ConcurrentSet()
        {
            backingDictionary = new ConcurrentDictionary<T, byte>();
        }

        public ConcurrentSet(IEqualityComparer<T> comparer)
        {
            backingDictionary = new ConcurrentDictionary<T, byte>(comparer);
        }

        public ConcurrentSet(IEnumerable<T> items)
        {
            backingDictionary = new ConcurrentDictionary<T, byte>(items.Select(item => new KeyValuePair<T, byte>(item, byte.MinValue)));
        }

        public ConcurrentSet(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            backingDictionary = new ConcurrentDictionary<T, byte>(items.Select(item => new KeyValuePair<T, byte>(item, byte.MinValue)), comparer);
        }

        public int Count => backingDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            backingDictionary.TryAdd(item, byte.MinValue);
        }

        public bool Remove(T item)
        {
            return backingDictionary.TryRemove(item, out var _);
        }

        public void Clear()
        {
            backingDictionary.Clear();
        }

        public bool Contains(T item)
        {
            return backingDictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            backingDictionary.Keys.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return backingDictionary.Select(pair => pair.Key).GetEnumerator();
        }

        bool ISet<T>.Add(T item)
        {
            return backingDictionary.TryAdd(item, byte.MinValue);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var value in other)
                Add(value);
        }

        #region Unsupported operations

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
