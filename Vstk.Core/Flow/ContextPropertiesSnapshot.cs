using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Vstk.Flow
{
    // TODO(iloktionov): Работает неэффективно (неизменямость за счет копирования массива при перезаписи/удалении, линейный перебор при поиске), рассматриваем как решение для прототипа.
    // TODO(iloktionov): В будущем можно будет заменить на ImmutableDictionary, если найдем способ втащить его, не отказываясь от таргетинга на netstandard и не внося внешних зависимостей.

    internal class ContextPropertiesSnapshot : IReadOnlyDictionary<string, object>
    {
        private const int defaultCapacity = 4;

        public static readonly ContextPropertiesSnapshot Empty = new ContextPropertiesSnapshot(0);

        private readonly ContextProperty[] properties;
        private readonly int count;

        public ContextPropertiesSnapshot()
            : this(defaultCapacity)
        {
        }

        public ContextPropertiesSnapshot(int capacity)
            : this(new ContextProperty[capacity], 0)
        {
        }

        private ContextPropertiesSnapshot(ContextProperty[] properties, int count)
        {
            this.properties = properties;
            this.count = count;
        }

        public int Count => count;

        public IEnumerable<string> Keys => this.Select(pair => pair.Key);

        public IEnumerable<object> Values => this.Select(pair => pair.Value);

        public object this[string key] => Find(key, out var value, out var _) ? value : throw new KeyNotFoundException();

        public bool ContainsKey(string key)
        {
            return Find(key, out var _, out var _);
        }

        public bool TryGetValue(string key, out object value)
        {
            return Find(key, out value, out var _);
        }

        public ContextPropertiesSnapshot Set(string key, object value)
        {
            ContextProperty[] newProperties;

            var newProperty = new ContextProperty(key, value);

            if (Find(key, out var oldValue, out var oldIndex))
            {
                if (Equals(value, oldValue))
                    return this;

                newProperties = ReallocateArray(properties.Length);
                newProperties[oldIndex] = newProperty;
                return new ContextPropertiesSnapshot(newProperties, count);
            }

            if (properties.Length == count)
            {
                newProperties = ReallocateArray(Math.Max(defaultCapacity, properties.Length*2));
                newProperties[count] = newProperty;
                return new ContextPropertiesSnapshot(newProperties, count + 1);
            }

            if (Interlocked.CompareExchange(ref properties[Count], newProperty, null) != null)
            {
                newProperties = ReallocateArray(properties.Length);
                newProperties[count] = newProperty;
                return new ContextPropertiesSnapshot(newProperties, count + 1);
            }

            return new ContextPropertiesSnapshot(properties, count + 1);
        }

        public ContextPropertiesSnapshot Remove(string key)
        {
            if (!Find(key, out var _, out var oldIndex))
                return this;

            if (oldIndex == count - 1)
                return new ContextPropertiesSnapshot(properties, count - 1);

            var newProperties = new ContextProperty[properties.Length - 1];

            if (oldIndex > 0)
                Array.Copy(properties, 0, newProperties, 0, oldIndex);

            Array.Copy(properties, oldIndex + 1, newProperties, oldIndex, count - oldIndex - 1);

            return new ContextPropertiesSnapshot(newProperties, count - 1);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (var i = 0; i < count; i++)
                yield return properties[i].ToKeyValuePair();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool Find(string key, out object value, out int index)
        {
            for (var i = 0; i < count; i++)
            {
                var property = properties[i];
                if (property.Key.Equals(key, StringComparison.Ordinal))
                {
                    index = i;
                    value = property.Value;
                    return true;
                }
            }

            index = -1;
            value = null;
            return false;
        }

        private ContextProperty[] ReallocateArray(int capacity)
        {
            var reallocated = new ContextProperty[capacity];

            Array.Copy(properties, 0, reallocated, 0, count);

            return reallocated;
        }
    }
}
