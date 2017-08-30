using System;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;
using Vostok.Clusterclient.Transport;
using Vostok.Logging;
using Xunit;

namespace Vostok.Clusterclient.Core
{
    public class ClusterClient_Tests
    {
        private readonly ILog log;

        public ClusterClient_Tests()
        {
            log = new ConsoleLog();
        }

        [Fact]
        public void Ctor_should_throw_an_error_when_created_with_incorrect_configuration()
        {
            Action action = () => new ClusterClient(log, _ => {});

            action.ShouldThrow<ClusterClientException>().Which.ShouldBePrinted();
        }

        [Fact]
        public void Should_use_cluser_provider_as_is_when_there_is_no_replicas_transform()
        {
            var clusterProvider = Substitute.For<IClusterProvider>();

            var clusterClient = new ClusterClient(
                log,
                config =>
                {
                    config.ClusterProvider = clusterProvider;
                    config.Transport = Substitute.For<ITransport>();
                });

            clusterClient.ClusterProvider.Should().BeSameAs(clusterProvider);
        }

        [Fact]
        public void Should_wrap_cluster_provider_with_transforming_facade_if_there_is_a_replicas_transform()
        {
            var clusterClient = new ClusterClient(
                log,
                config =>
                {
                    config.ClusterProvider = Substitute.For<IClusterProvider>();
                    config.Transport = Substitute.For<ITransport>();
                    config.ReplicaTransform = Substitute.For<IReplicaTransform>();
                });

            clusterClient.ClusterProvider.Should().BeOfType<TransformingClusterProvider>();
        }
    }
}
