namespace Vostok.Clusterclient.Ordering.Storage
{
    /// <summary>
    /// <para>Represents a high-level isolation scope of replica information storage.</para>
    /// <para>Further isolation is provided by value types and storage keys (see <see cref="IReplicaStorageProvider"/> for details).</para>
    /// </summary>
    public enum ReplicaStorageScope
    {
        /// <summary>
        /// There is one storage per value type and storage key in each process.
        /// </summary>
        Process,

        /// <summary>
        /// There is one storage per value type and storage key in each <see cref="ClusterClient"/> instance.
        /// </summary>
        Instance
    }
}
