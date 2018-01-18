using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;

namespace Vostok.ClusterClient.Tests.Core.Topology
{
    public class TransformingClusterProvider_Tests
    {
        private Uri replica1;
        private Uri replica2;
        private Uri replica3;
        private Uri replica4;
        private Uri replica5;
        private Uri replica6;

        private IClusterProvider baseProvider;
        private IReplicaTransform transform;
        private TransformingClusterProvider provider;

        [SetUp]
        public void SetUp()
        {
            replica1 = new Uri("http://replica1");
            replica2 = new Uri("http://replica2");
            replica3 = new Uri("http://replica3");
            replica4 = new Uri("http://replica4");
            replica5 = new Uri("http://replica5");
            replica6 = new Uri("http://replica6");

            baseProvider = Substitute.For<IClusterProvider>();
            baseProvider.GetCluster().Returns(new[] {replica1, replica2, replica3});

            transform = Substitute.For<IReplicaTransform>();
            transform.Transform(replica1).Returns(replica4);
            transform.Transform(replica2).Returns(replica5);
            transform.Transform(replica3).Returns(replica6);

            provider = new TransformingClusterProvider(baseProvider, transform);
        }

        [Test]
        public void Should_return_transformed_replicas_in_original_order()
        {
            provider.GetCluster().Should().Equal(replica4, replica5, replica6);
        }

        [Test]
        public void Should_cache_transformed_replicas_list()
        {
            var replicas1 = provider.GetCluster();
            var replicas2 = provider.GetCluster();

            replicas2.Should().BeSameAs(replicas1);
        }

        [Test]
        public void Should_react_to_changes_in_original_cluster()
        {
            baseProvider.GetCluster().Returns(new[] {replica3, replica2, replica1});

            provider.GetCluster().Should().Equal(replica6, replica5, replica4);

            baseProvider.GetCluster().Returns(new[] {replica2, replica1});

            provider.GetCluster().Should().Equal(replica5, replica4);

            baseProvider.GetCluster().Returns(new[] {replica1});

            provider.GetCluster().Should().Equal(replica4);
        }

        [Test]
        public void Should_drop_cache_when_original_cluster_changes()
        {
            var replicas1 = provider.GetCluster();

            baseProvider.GetCluster().Returns(new[] {replica1, replica2, replica3});

            var replicas2 = provider.GetCluster();

            replicas2.Should().NotBeSameAs(replicas1);
        }

        [Test]
        public void Should_pass_null_cluster_as_is()
        {
            baseProvider.GetCluster().Returns(null as IList<Uri>);

            provider.GetCluster().Should().BeNull();

            transform.ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public void Should_pass_empty_cluster_as_is()
        {
            baseProvider.GetCluster().Returns(new Uri[] {});

            provider.GetCluster().Should().BeEmpty();

            transform.ReceivedCalls().Should().BeEmpty();
        }
    }
}
