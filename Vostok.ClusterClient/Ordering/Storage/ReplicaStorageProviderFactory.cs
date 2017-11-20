namespace Vostok.Clusterclient.Ordering.Storage
{
    internal static class ReplicaStorageProviderFactory
    {
        private static readonly PerProcessReplicaStorageProvider sharedProvider = new PerProcessReplicaStorageProvider();

        public static IReplicaStorageProvider Create(ReplicaStorageScope scope)
        {
            return scope == ReplicaStorageScope.Process ? (IReplicaStorageProvider) sharedProvider : new PerInstanceReplicaStorageProvider();
        }
    }
}
