namespace Vostok.Clusterclient.Ordering.Storage
{
    internal static class ReplicaStorageProviderFactory
    {
        private static readonly PerProcessReplicaStorageProvider SharedProvider = new PerProcessReplicaStorageProvider();

        public static IReplicaStorageProvider Create(ReplicaStorageScope scope)
        {
            return scope == ReplicaStorageScope.Process ? (IReplicaStorageProvider) SharedProvider : new PerInstanceReplicaStorageProvider();
        }
    }
}
