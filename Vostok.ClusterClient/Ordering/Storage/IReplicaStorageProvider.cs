using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Ordering.Storage
{
    /// <summary>
    /// Represents an access layer for arbitrarily typed key-value replica information storages used by <see cref="IReplicaOrdering"/> and <see cref="Weighed.IReplicaWeightModifier"/> implementations.
    /// </summary>
    public interface IReplicaStorageProvider
    {
        /// <summary>
        /// <para>Returns a thread-safe dictionary which can be used to store/fetch typed objects for each replica.</para>
        /// <para>Concurrent access to these dictionaries is expected. It is therefore wise to use specialized <see cref="ConcurrentDictionary{TKey,TValue}"/> methods to avoid overwriting concurrent modifications.</para>
        /// <para>There are three levels of isolation to separate different storages from each other (from highest to lowest):</para>
        /// <list type="number">
        /// <item><see cref="ReplicaStorageScope"/>.</item>
        /// <item>Value type (<typeparamref name="TValue"/>, isolation inside same scope).</item>
        /// <item>Storage key (isolation inside same value type. You can use full type name of your implementation for this (or just a guid).</item>
        /// </list>
        /// </summary>
        /// <typeparam name="TValue">Type of the storage values.</typeparam>
        /// <param name="storageKey">A unique string used to isolate storages with same value types.</param>
        [NotNull]
        ConcurrentDictionary<Uri, TValue> Obtain<TValue>([CanBeNull] string storageKey = null);
    }
}
