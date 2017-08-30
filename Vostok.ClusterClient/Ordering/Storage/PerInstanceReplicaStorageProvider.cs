using System;
using System.Collections.Concurrent;

namespace Vostok.Clusterclient.Ordering.Storage
{
    internal class PerInstanceReplicaStorageProvider : IReplicaStorageProvider
    {
        private readonly ConcurrentDictionary<Type, object> containers;

        public PerInstanceReplicaStorageProvider()
        {
            containers = new ConcurrentDictionary<Type, object>();
        }

        public ConcurrentDictionary<Uri, TValue> Obtain<TValue>(string storageKey = null)
        {
            return ObtainContainer<TValue>().Obtain(storageKey);
        }

        private ReplicaStorageContainer<TValue> ObtainContainer<TValue>()
        {
            return (ReplicaStorageContainer<TValue>) containers.GetOrAdd(typeof (TValue), _ => new ReplicaStorageContainer<TValue>());
        }
    }
}
