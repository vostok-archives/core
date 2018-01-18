using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Retry;
using Vostok.ClusterClient.Tests.Helpers;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class RequestRetryModule_Tests
    {
        private const int maxAttempts = 5;

        private int nextModuleCalls;
        private ClusterResult result;
        private IRequestContext context;
        private IRetryPolicy retryPolicy;
        private IRetryStrategy retryStrategy;
        private RequestRetryModule module;
        private Request request;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("foo/bar");
            result = new ClusterResult(ClusterResultStatus.ReplicasExhausted, new List<ReplicaResult>(), null, request);
            nextModuleCalls = 0;

            var log = new ConsoleLog();
            var budget = Budget.Infinite;

            context = Substitute.For<IRequestContext>();
            context.Budget.Returns(budget);
            context.Log.Returns(log);
            context.Request.Returns(request);

            retryPolicy = Substitute.For<IRetryPolicy>();
            retryPolicy.NeedToRetry(Arg.Any<IList<ReplicaResult>>()).Returns(true);

            retryStrategy = Substitute.For<IRetryStrategy>();
            retryStrategy.AttemptsCount.Returns(maxAttempts);
            retryStrategy.GetRetryDelay(Arg.Any<int>()).Returns(TimeSpan.Zero);

            module = new RequestRetryModule(retryPolicy, retryStrategy);
        }

        [Test]
        public void Should_use_all_available_attempts_when_retry_is_possible_and_requested()
        {
            Execute().Should().BeSameAs(result);

            nextModuleCalls.Should().Be(maxAttempts);
        }

        [Test]
        public void Should_query_retry_possibility_before_each_additional_attempt()
        {
            Execute();

            retryPolicy.Received(4).NeedToRetry(result.ReplicaResults);
        }

        [Test]
        public void Should_query_retry_delay_before_each_additional_attempt()
        {
            Execute();

            retryStrategy.Received(4).GetRetryDelay(Arg.Any<int>());

            retryStrategy.Received().GetRetryDelay(1);
            retryStrategy.Received().GetRetryDelay(2);
            retryStrategy.Received().GetRetryDelay(3);
            retryStrategy.Received().GetRetryDelay(4);
        }

        
        [TestCase(ClusterResultStatus.Success)]
        [TestCase(ClusterResultStatus.TimeExpired)]
        [TestCase(ClusterResultStatus.ReplicasNotFound)]
        [TestCase(ClusterResultStatus.IncorrectArguments)]
        [TestCase(ClusterResultStatus.UnexpectedException)]
        public void Should_not_retry_if_result_has_given_status(ClusterResultStatus status)
        {
            result = new ClusterResult(status, new List<ReplicaResult>(), null, request);

            Execute().Should().BeSameAs(result);

            nextModuleCalls.Should().Be(1);
        }

        [Test]
        public void Should_not_retry_if_time_budget_has_expired()
        {
            context.Budget.Returns(Budget.Expired);

            Execute().Should().BeSameAs(result);

            nextModuleCalls.Should().Be(1);
        }

        [Test]
        public void Should_not_retry_if_retry_strategy_attempts_count_is_insufficient()
        {
            retryStrategy.AttemptsCount.Returns(1);

            Execute().Should().BeSameAs(result);

            nextModuleCalls.Should().Be(1);
        }

        [Test]
        public void Should_not_retry_if_retry_policy_forbids_it()
        {
            retryPolicy.NeedToRetry(Arg.Any<IList<ReplicaResult>>()).Returns(false);

            Execute().Should().BeSameAs(result);

            nextModuleCalls.Should().Be(1);
        }

        [Test]
        public void Should_reset_replica_results_in_native_context_implementation()
        {
            var contextImpl = new RequestContext(request, null, Budget.Infinite, context.Log, CancellationToken.None, null, int.MaxValue);

            contextImpl.SetReplicaResult(new ReplicaResult(new Uri("http://replica1"), Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero));
            contextImpl.SetReplicaResult(new ReplicaResult(new Uri("http://replica2"), Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero));

            context = contextImpl;

            Execute();

            contextImpl.FreezeReplicaResults().Should().BeEmpty();
        }

        [Test]
        public void Should_check_cancellation_token_before_each_attempt()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            tokenSource.Cancel();

            context.CancellationToken.Returns(CancellationToken.None, CancellationToken.None, token);

            Action action = () => Execute();

            action.ShouldThrow<OperationCanceledException>();

            nextModuleCalls.Should().Be(2);
        }

        private ClusterResult Execute()
        {
            return module.ExecuteAsync(
                    context,
                    ctx =>
                    {
                        nextModuleCalls++;
                        return Task.FromResult(result);
                    })
                .GetAwaiter()
                .GetResult();
        }
    }
}
