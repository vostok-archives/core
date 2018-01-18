using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transforms;
using Vostok.Clusterclient.Transport;
using Vostok.ClusterClient.Tests.Helpers;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core
{
    public class ClusterClient_Tests
    {
        private ILog log;

        [SetUp]
        public void SetUp()
        {
            log = new ConsoleLog();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_created_with_incorrect_configuration()
        {
            Action action = () => new Clusterclient.ClusterClient(log, _ => {});

            action.ShouldThrow<ClusterClientException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_use_cluster_provider_as_is_when_there_is_no_replicas_transform()
        {
            var clusterProvider = Substitute.For<IClusterProvider>();

            var clusterClient = new Clusterclient.ClusterClient(
                log,
                config =>
                {
                    config.ClusterProvider = clusterProvider;
                    config.Transport = Substitute.For<ITransport>();
                });

            clusterClient.ClusterProvider.Should().BeSameAs(clusterProvider);
        }

        [Test]
        public void Should_wrap_cluster_provider_with_transforming_facade_if_there_is_a_replicas_transform()
        {
            var clusterClient = new Clusterclient.ClusterClient(
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
