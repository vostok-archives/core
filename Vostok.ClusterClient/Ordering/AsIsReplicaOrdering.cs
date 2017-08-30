using System;
using System.Collections.Generic;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;

namespace Vostok.Clusterclient.Ordering
{
    /// <summary>
    /// Represents an ordering which never changes replicas order.
    /// </summary>
    public class AsIsReplicaOrdering : IReplicaOrdering
    {
        public IEnumerable<Uri> Order(IList<Uri> replicas, IReplicaStorageProvider storageProvider, Request request)
        {
            return replicas;
        }

        public void Learn(ReplicaResult result, IReplicaStorageProvider storageProvider)
        {
        }
    }
}
