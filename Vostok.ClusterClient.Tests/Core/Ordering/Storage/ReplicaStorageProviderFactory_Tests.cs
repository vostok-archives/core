using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Ordering.Storage;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Storage
{
    public class ReplicaStorageProviderFactory_Tests
    {
        [Test]
        public void Should_return_per_process_provider_for_process_scope()
        {
            ReplicaStorageProviderFactory.Create(ReplicaStorageScope.Process).Should().BeOfType<PerProcessReplicaStorageProvider>();
        }

        [Test]
        public void Should_return_per_instance_provider_for_instance_scope()
        {
            ReplicaStorageProviderFactory.Create(ReplicaStorageScope.Instance).Should().BeOfType<PerInstanceReplicaStorageProvider>();
        }

        [Test]
        public void Should_always_return_same_per_process_provider()
        {
            var provider1 = ReplicaStorageProviderFactory.Create(ReplicaStorageScope.Process);
            var provider2 = ReplicaStorageProviderFactory.Create(ReplicaStorageScope.Process);

            provider2.Should().BeSameAs(provider1);
        }

        [Test]
        public void Should_always_return_different_per_instance_providers()
        {
            var provider1 = ReplicaStorageProviderFactory.Create(ReplicaStorageScope.Instance);
            var provider2 = ReplicaStorageProviderFactory.Create(ReplicaStorageScope.Instance);

            provider2.Should().NotBeSameAs(provider1);
        }
    }
}
