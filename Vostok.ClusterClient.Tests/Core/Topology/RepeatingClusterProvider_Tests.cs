using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Topology;

namespace Vostok.ClusterClient.Tests.Core.Topology
{
    public class RepeatingClusterProvider_Tests
    {
        private Uri replica1;
        private Uri replica2;

        private IClusterProvider baseProvider;
        private RepeatingClusterProvider provider;

        [SetUp]
        public void SetUp()
        {
            replica1 = new Uri("http://replica1");
            replica2 = new Uri("http://replica2");

            baseProvider = Substitute.For<IClusterProvider>();
            baseProvider.GetCluster().Returns(new[] {replica1, replica2});

            provider = new RepeatingClusterProvider(baseProvider, 3);
        }

        [Test]
        public void Should_repeat_base_provider_replicas()
        {
            provider.GetCluster().Should().Equal(replica1, replica2, replica1, replica2, replica1, replica2);
        }

        [Test]
        public void Should_cache_repeated_replicas_list()
        {
            var replicas1 = provider.GetCluster();
            var replicas2 = provider.GetCluster();

            replicas2.Should().BeSameAs(replicas1);
        }

        [Test]
        public void Should_react_to_changes_in_original_cluster()
        {
            baseProvider.GetCluster().Returns(new[] {replica1});

            provider.GetCluster().Should().Equal(replica1, replica1, replica1);

            baseProvider.GetCluster().Returns(new[] {replica2});

            provider.GetCluster().Should().Equal(replica2, replica2, replica2);
        }

        [Test]
        public void Should_drop_cache_when_original_cluster_changes()
        {
            var replicas1 = provider.GetCluster();

            baseProvider.GetCluster().Returns(new[] {replica1});

            var replicas2 = provider.GetCluster();

            replicas2.Should().NotBeSameAs(replicas1);
        }

        [Test]
        public void Should_pass_null_cluster_as_is()
        {
            baseProvider.GetCluster().Returns(null as IList<Uri>);

            provider.GetCluster().Should().BeNull();
        }

        [Test]
        public void Should_pass_empty_cluster_as_is()
        {
            baseProvider.GetCluster().Returns(new Uri[] {});

            provider.GetCluster().Should().BeEmpty();
        }
    }
}
