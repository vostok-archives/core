using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;

namespace Vostok.Clusterclient.Ordering.Weighed
{
    internal interface IReplicaWeightCalculator
    {
        double GetWeight(
            [NotNull] Uri replica, 
            [NotNull] IList<Uri> allReplicas, 
            [NotNull] IReplicaStorageProvider storageProvider, 
            [NotNull] Request request);
    }
}
