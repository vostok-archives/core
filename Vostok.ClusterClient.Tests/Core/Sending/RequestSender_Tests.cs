using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Sending;
using Vostok.Clusterclient.Transport;
using Vostok.ClusterClient.Tests.Helpers;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Sending
{
    public class RequestSender_Tests
    {
        private Uri replica;
        private Request relativeRequest;
        private Response response;
        private TimeSpan timeout;

        private IClusterClientConfiguration configuration;
        private IReplicaStorageProvider storageProvider;
        private IResponseClassifier responseClassifier;
        private IRequestConverter requestConverter;
        private ITransport transport;
        private ILog log;

        private RequestSender sender;
        private Request absoluteRequest;

        [SetUp]
        public void SetUp()
        {
            replica = new Uri("http://replica/");
            relativeRequest = Request.Get("foo/bar");
            absoluteRequest = Request.Get("http://replica/foo/bar");
            response = new Response(ResponseCode.Ok);
            timeout = 5.Seconds();

            log = Substitute.For<ILog>();
            log
                .When(l => l.Log(Arg.Any<LogEvent>()))
                .Do(info => new ConsoleLog().Log(info.Arg<LogEvent>()));

            configuration = Substitute.For<IClusterClientConfiguration>();
            configuration.ResponseCriteria.Returns(new List<IResponseCriterion> {Substitute.For<IResponseCriterion>()});
            configuration.LogReplicaRequests.Returns(true);
            configuration.LogReplicaResults.Returns(true);
            configuration.ReplicaOrdering.Returns(Substitute.For<IReplicaOrdering>());
            configuration.Log.Returns(log);

            storageProvider = Substitute.For<IReplicaStorageProvider>();

            responseClassifier = Substitute.For<IResponseClassifier>();
            responseClassifier.Decide(Arg.Any<Response>(), Arg.Any<IList<IResponseCriterion>>()).Returns(ResponseVerdict.Accept);

            requestConverter = Substitute.For<IRequestConverter>();
            requestConverter.TryConvertToAbsolute(relativeRequest, replica).Returns(_ => absoluteRequest);

            transport = Substitute.For<ITransport>();
            transport.SendAsync(Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Returns(_ => response);

            sender = new RequestSender(configuration, storageProvider, responseClassifier, requestConverter, transport);
        }

        [Test]
        public void Should_convert_relative_request_to_absolute()
        {
            Send();

            requestConverter.Received(1).TryConvertToAbsolute(relativeRequest, replica);
        }

        [Test]
        public void Should_send_request_with_transport_when_request_conversion_succeeds()
        {
            var tokenSource = new CancellationTokenSource();

            Send(tokenSource.Token);

            transport.Received(1).SendAsync(absoluteRequest, timeout, tokenSource.Token);
        }

        [Test]
        public void Should_not_touch_transport_when_request_conversion_fails()
        {
            absoluteRequest = null;

            Send();

            transport.ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public void Should_return_unknown_response_when_request_conversion_fails()
        {
            absoluteRequest = null;

            Send().Response.Should().BeSameAs(Responses.Unknown);
        }

        [Test]
        public void Should_return_unknown_failure_response_when_transport_throws_an_exception()
        {
            transport.SendAsync(Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Throws(new Exception("Fail!"));

            Send().Response.Should().BeSameAs(Responses.UnknownFailure);
        }

        [Test]
        public void Should_not_catch_cancellation_exceptions()
        {
            transport.SendAsync(Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).Throws(new OperationCanceledException());

            Action action = () => Send();

            action.ShouldThrow<OperationCanceledException>();
        }

        [Test]
        public void Should_throw_a_cancellation_exception_when_transport_returns_canceled_response()
        {
            transport.SendAsync(Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()).ReturnsTask(Responses.Canceled);

            Action action = () => Send();

            action.ShouldThrow<OperationCanceledException>();
        }

        [Test]
        public void Should_classify_response_from_replica()
        {
            Send();

            responseClassifier.Received().Decide(response, configuration.ResponseCriteria);
        }

        [Test]
        public void Should_report_replica_result_to_replica_ordering()
        {
            Send();

            configuration.ReplicaOrdering.Received(1).Learn(Arg.Any<ReplicaResult>(), storageProvider);
        }

        [Test]
        public void Should_return_result_with_correct_replica()
        {
            Send().Replica.Should().BeSameAs(replica);
        }

        [Test]
        public void Should_return_result_with_correct_response()
        {
            Send().Response.Should().BeSameAs(response);
        }

        
        [TestCase(ResponseVerdict.Accept)]
        [TestCase(ResponseVerdict.Reject)]
        public void Should_return_result_with_correct_verdict(ResponseVerdict verdict)
        {
            responseClassifier.Decide(response, configuration.ResponseCriteria).Returns(verdict);

            Send().Verdict.Should().Be(verdict);
        }

        [Test]
        public void Should_log_requests_and_results_if_asked_to()
        {
            Send();

            log.Received(2).Log(Arg.Is<LogEvent>(evt => evt.Level == LogLevel.Info));
        }

        [Test]
        public void Should_not_log_requests_and_results_if_not_asked_to()
        {
            configuration.LogReplicaRequests.Returns(false);
            configuration.LogReplicaResults.Returns(false);

            Send();

            log.ReceivedCalls().Should().BeEmpty();
        }

        private ReplicaResult Send(CancellationToken token = default(CancellationToken))
        {
            return sender.SendToReplicaAsync(replica, relativeRequest, timeout, token).GetAwaiter().GetResult();
        }
    }
}
