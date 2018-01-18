using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Ordering;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Sending;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Topology;
using Vostok.ClusterClient.Tests.Helpers;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class RequestExecutionModule_Tests
    {
        private Uri replica1;
        private Uri replica2;
        private Response response1;
        private Response response2;
        private Response selectedResponse;
        private ReplicaResult result1;
        private ReplicaResult result2;

        private IClusterProvider clusterProvider;
        private IReplicaOrdering replicaOrdering;
        private IResponseSelector responseSelector;
        private IReplicaStorageProvider storageProvider;
        private IRequestSender requestSender;
        private IClusterResultStatusSelector resultStatusSelector;
        private RequestContext context;
        private RequestExecutionModule module;

        [SetUp]
        public void SetUp()
        {
            replica1 = new Uri("http://replica1");
            replica2 = new Uri("http://replica2");

            response1 = new Response(ResponseCode.Ok);
            response2 = new Response(ResponseCode.Ok);
            selectedResponse = new Response(ResponseCode.Ok);

            result1 = new ReplicaResult(replica1, response1, ResponseVerdict.DontKnow, TimeSpan.Zero);
            result2 = new ReplicaResult(replica2, response2, ResponseVerdict.DontKnow, TimeSpan.Zero);

            var log = new ConsoleLog();

            context = new RequestContext(Request.Get("foo/bar"), Substitute.For<IRequestStrategy>(), Budget.Infinite, log, CancellationToken.None, null, int.MaxValue);
            // ReSharper disable AssignNullToNotNullAttribute
            context.Strategy.SendAsync(null, null, null, null, 0, default(CancellationToken))
                // ReSharper restore AssignNullToNotNullAttribute
                .ReturnsForAnyArgs(
                    async info =>
                    {
                        var replicas = info.Arg<IEnumerable<Uri>>();
                        var sender = info.Arg<IRequestSender>();

                        foreach (var replica in replicas)
                        {
                            await sender.SendToReplicaAsync(replica, context.Request, TimeSpan.Zero, CancellationToken.None);
                        }
                    });

            clusterProvider = Substitute.For<IClusterProvider>();
            clusterProvider.GetCluster().Returns(new[] {replica1, replica2});

            requestSender = Substitute.For<IRequestSender>();
            requestSender.SendToReplicaAsync(replica1, Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).ReturnsTask(_ => result1);
            requestSender.SendToReplicaAsync(replica2, Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).ReturnsTask(_ => result2);

            replicaOrdering = Substitute.For<IReplicaOrdering>();
            // ReSharper disable AssignNullToNotNullAttribute
            replicaOrdering.Order(null, null, null).ReturnsForAnyArgs(info => info.Arg<IList<Uri>>().Reverse());
            // ReSharper restore AssignNullToNotNullAttribute

            responseSelector = Substitute.For<IResponseSelector>();
            // ReSharper disable once AssignNullToNotNullAttribute
            responseSelector.Select(null).ReturnsForAnyArgs(_ => selectedResponse);

            resultStatusSelector = Substitute.For<IClusterResultStatusSelector>();
            // ReSharper disable AssignNullToNotNullAttribute
            resultStatusSelector.Select(null, null).ReturnsForAnyArgs(ClusterResultStatus.Success);
            // ReSharper restore AssignNullToNotNullAttribute

            storageProvider = Substitute.For<IReplicaStorageProvider>();
            module = new RequestExecutionModule(
                clusterProvider,
                replicaOrdering,
                responseSelector,
                storageProvider,
                requestSender,
                resultStatusSelector);
        }

        [Test]
        public void Should_return_no_replicas_result_when_cluster_provider_returns_null()
        {
            clusterProvider.GetCluster().Returns(null as IList<Uri>);

            Execute().Status.Should().Be(ClusterResultStatus.ReplicasNotFound);
        }

        [Test]
        public void Should_return_no_replicas_result_when_cluster_provider_returns_an_empty_list()
        {
            clusterProvider.GetCluster().Returns(new List<Uri>());

            Execute().Status.Should().Be(ClusterResultStatus.ReplicasNotFound);
        }

        [Test]
        public void Should_order_replicas_obtained_from_cluster_provider()
        {
            Execute();

            replicaOrdering.Received().Order(Arg.Is<IList<Uri>>(urls => urls.SequenceEqual(new[] {replica1, replica2})), storageProvider, context.Request);
        }

        [Test]
        public void Should_invoke_request_strategy_with_correct_parameters()
        {
            Execute();

            context.Strategy.Received().SendAsync(context.Request, Arg.Any<ContextualRequestSender>(), context.Budget, Arg.Is<IEnumerable<Uri>>(urls => urls.SequenceEqual(new[] {replica2, replica1})), 2, context.CancellationToken);
        }

        [Test]
        public void Should_invoke_request_strategy_with_correct_parameters_when_limiting_replicas_count()
        {
            context.MaximumReplicasToUse = 1;

            Execute();

            context.Strategy.Received().SendAsync(context.Request, Arg.Any<ContextualRequestSender>(), context.Budget, Arg.Is<IEnumerable<Uri>>(urls => urls.SequenceEqual(new[] {replica2})), 1, context.CancellationToken);
        }

        [Test]
        public void Should_check_cancellation_token_after_invoking_request_strategy()
        {
            var tokenSource = new CancellationTokenSource();

            tokenSource.Cancel();

            context = new RequestContext(context.Request, context.Strategy, context.Budget, context.Log, tokenSource.Token, null, int.MaxValue);

            Action action = () => Execute();

            action.ShouldThrow<OperationCanceledException>();

            responseSelector.ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public void Should_select_response_based_on_all_replica_results()
        {
            Execute();

            responseSelector.Received().Select(Arg.Is<IList<ReplicaResult>>(results => results.SequenceEqual(new[] {result2, result1})));
        }

        [Test]
        public void Should_return_a_result_with_selected_response()
        {
            Execute().Response.Should().BeSameAs(selectedResponse);
        }

        [Test]
        public void Should_return_a_result_with_all_replica_results()
        {
            Execute().ReplicaResults.Should().Equal(result2, result1);
        }

        
        [TestCase(ClusterResultStatus.Success)]
        [TestCase(ClusterResultStatus.TimeExpired)]
        [TestCase(ClusterResultStatus.ReplicasExhausted)]
        public void Should_return_a_result_with_status_selected_by_result_status_selector(ClusterResultStatus status)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            resultStatusSelector.Select(null, null).ReturnsForAnyArgs(status);
            // ReSharper restore AssignNullToNotNullAttribute

            Execute().Status.Should().Be(status);
        }

        private ClusterResult Execute()
        {
            return module.ExecuteAsync(context, _ => { throw new NotSupportedException(); }).GetAwaiter().GetResult();
        }
    }
}
