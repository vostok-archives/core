using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class ReplicaBudgetingModule_Tests
    {
        private const int minimumRequests = 50;

        private Uri replica1;
        private Uri replica2;
        private Request request;
        private ClusterResult oneReplicaResult;
        private ClusterResult twoReplicasResult;
        private IRequestContext context;

        private ReplicaBudgetingOptions options;
        private ReplicaBudgetingModule module;

        [SetUp]
        public void SetUp()
        {
            replica1 = new Uri("http://replica");
            replica2 = new Uri("http://replica");
            request = Request.Get("foo/bar");

            oneReplicaResult = new ClusterResult(
                ClusterResultStatus.ReplicasExhausted,
                new[]
                {
                    new ReplicaResult(replica1, Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero)
                },
                null,
                request);

            twoReplicasResult = new ClusterResult(
                ClusterResultStatus.ReplicasExhausted,
                new[]
                {
                    new ReplicaResult(replica1, Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero),
                    new ReplicaResult(replica2, Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero)
                },
                null,
                request);

            context = Substitute.For<IRequestContext>();
            context.Log.Returns(new SilentLog());
            context.MaximumReplicasToUse.Returns(int.MaxValue);

            options = new ReplicaBudgetingOptions(Guid.NewGuid().ToString(), 1, minimumRequests);
            module = new ReplicaBudgetingModule(options);
        }

        [Test]
        public void Should_increment_requests_counter_on_every_request()
        {
            for (var i = 1; i <= 10; i++)
            {
                Execute(oneReplicaResult);

                module.Requests.Should().Be(i);
            }
        }

        [Test]
        public void Should_add_replica_results_count_to_replicas_counter_on_every_request()
        {
            Execute(oneReplicaResult);

            module.Replicas.Should().Be(1);

            Execute(twoReplicasResult);

            module.Replicas.Should().Be(3);

            Execute(oneReplicaResult);
            Execute(oneReplicaResult);

            module.Replicas.Should().Be(5);
        }

        [Test]
        public void Should_correctly_compute_replicas_to_requests_ratio()
        {
            module.Ratio.Should().Be(0.0);

            SpinWithOneReplica(10);

            module.Ratio.Should().Be(1.0);

            SpinWithTwoReplicas(10);

            module.Ratio.Should().Be(1.5);
        }

        [Test]
        public void Should_not_limit_used_replicas_while_ratio_is_below_critical_value()
        {
            SpinWithOneReplica(100);

            context.DidNotReceive().MaximumReplicasToUse = 1;
        }

        [Test]
        public void Should_not_limit_used_replicas_until_minimum_requests_count_is_reached()
        {
            SpinWithTwoReplicas(minimumRequests - 1);

            context.DidNotReceive().MaximumReplicasToUse = 1;
        }

        [Test]
        public void Should_limit_used_replicas_when_ratio_is_above_critical_value()
        {
            SpinWithTwoReplicas(minimumRequests*2);

            context.ClearReceivedCalls();

            Execute(oneReplicaResult);

            context.Received(1).MaximumReplicasToUse = 1;
        }

        private void SpinWithOneReplica(int count)
        {
            for (var i = 0; i < count; i++)
                Execute(oneReplicaResult);
        }

        private void SpinWithTwoReplicas(int count)
        {
            for (var i = 0; i < count; i++)
                Execute(twoReplicasResult);
        }

        private void Execute(ClusterResult result)
        {
            module.ExecuteAsync(context, _ => Task.FromResult(result)).GetAwaiter().GetResult().Should().BeSameAs(result);
        }
    }
}
