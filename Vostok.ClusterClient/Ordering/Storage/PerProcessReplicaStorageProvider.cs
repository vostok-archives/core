using System;
using System.Collections.Concurrent;

namespace Vostok.Clusterclient.Ordering.Storage
{
    internal class PerProcessReplicaStorageProvider : IReplicaStorageProvider
    {
        public ConcurrentDictionary<Uri, TValue> Obtain<TValue>(string storageKey = null)
        {
            return ReplicaStorageContainer<TValue>.Shared.Obtain(storageKey);
        }
    }
}
