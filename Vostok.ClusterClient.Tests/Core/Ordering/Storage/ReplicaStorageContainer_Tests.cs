using FluentAssertions;
using Vostok.Clusterclient.Ordering.Storage;
using Xunit;

namespace Vostok.Clusterclient.Core.Ordering.Storage
{
    public class ReplicaStorageContainer_Tests
    {
        private readonly ReplicaStorageContainer<int> container;

        public ReplicaStorageContainer_Tests()
        {
            container = new ReplicaStorageContainer<int>();
        }

        [Fact]
        public void Obtain_should_always_return_same_storage_for_null_storage_key()
        {
            var storage1 = container.Obtain(null);
            var storage2 = container.Obtain(null);

            storage2.Should().BeSameAs(storage1);
        }

        [Fact]
        public void Obtain_should_always_return_same_storage_for_same_storage_key()
        {
            var storage1 = container.Obtain("key");
            var storage2 = container.Obtain("key");

            storage2.Should().BeSameAs(storage1);
        }

        [Fact]
        public void Obtain_should_return_different_storages_for_null_and_empty_storage_keys()
        {
            var storage1 = container.Obtain(null);
            var storage2 = container.Obtain(string.Empty);

            storage2.Should().NotBeSameAs(storage1);
        }

        [Fact]
        public void Obtain_should_return_different_storages_for_different_non_null_storage_keys()
        {
            var storage1 = container.Obtain("key1");
            var storage2 = container.Obtain("key2");

            storage2.Should().NotBeSameAs(storage1);
        }
    }
}
