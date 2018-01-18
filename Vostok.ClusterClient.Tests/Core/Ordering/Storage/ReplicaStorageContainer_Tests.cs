using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Ordering.Storage;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Storage
{
    public class ReplicaStorageContainer_Tests
    {
        private ReplicaStorageContainer<int> container;

        [SetUp]
        public void SetUp()
        {
            container = new ReplicaStorageContainer<int>();
        }

        [Test]
        public void Obtain_should_always_return_same_storage_for_null_storage_key()
        {
            var storage1 = container.Obtain(null);
            var storage2 = container.Obtain(null);

            storage2.Should().BeSameAs(storage1);
        }

        [Test]
        public void Obtain_should_always_return_same_storage_for_same_storage_key()
        {
            var storage1 = container.Obtain("key");
            var storage2 = container.Obtain("key");

            storage2.Should().BeSameAs(storage1);
        }

        [Test]
        public void Obtain_should_return_different_storages_for_null_and_empty_storage_keys()
        {
            var storage1 = container.Obtain(null);
            var storage2 = container.Obtain(string.Empty);

            storage2.Should().NotBeSameAs(storage1);
        }

        [Test]
        public void Obtain_should_return_different_storages_for_different_non_null_storage_keys()
        {
            var storage1 = container.Obtain("key1");
            var storage2 = container.Obtain("key2");

            storage2.Should().NotBeSameAs(storage1);
        }
    }
}
