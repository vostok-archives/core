using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Transport;
using Vostok.ClusterClient.Tests.Helpers;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class AbsoluteUrlSenderModule_Tests
    {
        private ITransport transport;
        private IResponseClassifier responseClassifier;
        private IList<IResponseCriterion> responseCriteria;
        private IClusterResultStatusSelector resultStatusSelector;

        private IRequestContext context;

        private AbsoluteUrlSenderModule module;
        private Request request;
        private Response response;

        [SetUp]
        public void Setup()
        {
            request = Request.Get("http://foo/bar");
            response = new Response(ResponseCode.Ok);

            var budget = Budget.WithRemaining(5.Seconds());

            context = Substitute.For<IRequestContext>();
            context.Request.Returns(_ => request);
            context.Budget.Returns(_ => budget);

            transport = Substitute.For<ITransport>();
            transport.SendAsync(Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).ReturnsTask(_ => response);

            responseCriteria = new List<IResponseCriterion>();
            responseClassifier = Substitute.For<IResponseClassifier>();
            responseClassifier.Decide(Arg.Any<Response>(), Arg.Any<IList<IResponseCriterion>>()).Returns(ResponseVerdict.Accept);

            resultStatusSelector = Substitute.For<IClusterResultStatusSelector>();
            // ReSharper disable AssignNullToNotNullAttribute
            resultStatusSelector.Select(null, null).ReturnsForAnyArgs(ClusterResultStatus.Success);
            // ReSharper restore AssignNullToNotNullAttribute

            module = new AbsoluteUrlSenderModule(transport, responseClassifier, responseCriteria, resultStatusSelector);
        }

        [Test]
        public void Should_delegate_to_next_module_when_request_url_is_relative()
        {
            request = Request.Get("foo/bar");

            var result = new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), response, request);

            Execute(result).Should().BeSameAs(result);

            transport.ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public void Should_send_request_using_transport_directly_if_url_is_absolute()
        {
            Execute();

            transport.Received().SendAsync(request, 5.Seconds(), context.CancellationToken);
        }

        [Test]
        public void Should_return_canceled_result_if_transport_returns_a_canceled_response()
        {
            response = new Response(ResponseCode.Canceled);

            Execute().Status.Should().Be(ClusterResultStatus.Canceled);
        }

        [Test]
        public void Should_classify_response_to_obtain_a_verdict()
        {
            Execute();

            responseClassifier.Received().Decide(response, responseCriteria);
        }

        
        [TestCase(ClusterResultStatus.Success)]
        [TestCase(ClusterResultStatus.TimeExpired)]
        [TestCase(ClusterResultStatus.ReplicasExhausted)]
        public void Should_return_result_with_status_given_by_result_status_selector(ClusterResultStatus status)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            resultStatusSelector.Select(null, null).ReturnsForAnyArgs(status);
            // ReSharper restore AssignNullToNotNullAttribute

            Execute().Status.Should().Be(status);
        }

        [Test]
        public void Should_return_result_with_received_response_from_transport()
        {
            Execute().Response.Should().BeSameAs(response);
        }

        [Test]
        public void Should_return_result_with_a_single_correct_replica_result()
        {
            var replicaResult = Execute().ReplicaResults.Should().ContainSingle().Which;

            replicaResult.Replica.Should().BeSameAs(request.Url);
            replicaResult.Response.Should().BeSameAs(response);
            replicaResult.Verdict.Should().Be(ResponseVerdict.Accept);
        }

        private ClusterResult Execute(ClusterResult result = null)
        {
            return module.ExecuteAsync(context, _ => Task.FromResult(result)).GetAwaiter().GetResult();
        }
    }
}
