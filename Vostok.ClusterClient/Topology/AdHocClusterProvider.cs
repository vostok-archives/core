using System;
using System.Collections.Generic;

namespace Vostok.Clusterclient.Topology
{
    /// <summary>
    /// Represents a cluster provider which uses an external delegate to provide replica urls.
    /// </summary>
    public class AdHocClusterProvider : IClusterProvider
    {
        private readonly Func<IList<Uri>> replicasProvider;

        public AdHocClusterProvider(Func<IList<Uri>> replicasProvider)
        {
            this.replicasProvider = replicasProvider;
        }

        public IList<Uri> GetCluster()
        {
            return replicasProvider();
        }
    }
}
