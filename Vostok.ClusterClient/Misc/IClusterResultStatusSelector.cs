using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Misc
{
    internal interface IClusterResultStatusSelector
    {
        ClusterResultStatus Select([NotNull] IList<ReplicaResult> results, [NotNull] IRequestTimeBudget budget);
    }
}
