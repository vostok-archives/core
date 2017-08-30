using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Ordering.Storage
{
    internal class ReplicaStorageContainer<TValue>
    {
        public static readonly ReplicaStorageContainer<TValue> Shared = new ReplicaStorageContainer<TValue>();

        private readonly ConcurrentDictionary<Uri, TValue> commonStorage;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Uri, TValue>> keyedStorages;

        public ReplicaStorageContainer()
        {
            commonStorage = new ConcurrentDictionary<Uri, TValue>();
            keyedStorages = new ConcurrentDictionary<string, ConcurrentDictionary<Uri, TValue>>();
        }

        [NotNull]
        public ConcurrentDictionary<Uri, TValue> Obtain([CanBeNull] string storageKey)
        {
            return storageKey == null ? commonStorage : keyedStorages.GetOrAdd(storageKey, _ => new ConcurrentDictionary<Uri, TValue>());
        }
    }
}
