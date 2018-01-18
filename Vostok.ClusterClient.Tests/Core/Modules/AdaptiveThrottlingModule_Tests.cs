using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class AdaptiveThrottlingModule_Tests
    {
        private const int minimumRequests = 50;
        private const double criticalRatio = 2.0;
        private const double probabilityCap = 0.8;

        private Uri replica;
        private Request request;
        private ClusterResult acceptedResult;
        private ClusterResult rejectedResult;
        private IRequestContext context;

        private AdaptiveThrottlingOptions options;
        private AdaptiveThrottlingModule module;

        [SetUp]
        public void Setup()
        {
            replica = new Uri("http://replica");
            request = Request.Get("foo/bar");
            acceptedResult = new ClusterResult(ClusterResultStatus.Success, new[] {new ReplicaResult(replica, new Response(ResponseCode.Accepted), ResponseVerdict.Accept, TimeSpan.Zero)}, null, request);
            rejectedResult = new ClusterResult(ClusterResultStatus.ReplicasExhausted, new[] {new ReplicaResult(replica, new Response(ResponseCode.TooManyRequests), ResponseVerdict.Reject, TimeSpan.Zero)}, null, request);

            context = Substitute.For<IRequestContext>();
            context.Log.Returns(new SilentLog());

            options = new AdaptiveThrottlingOptions(Guid.NewGuid().ToString(), 1, minimumRequests);
            module = new AdaptiveThrottlingModule(options);
        }

        [Test]
        public void Should_increment_requests_and_accepts_on_accepted_results()
        {
            for (var i = 1; i <= 10; i++)
            {
                Execute(acceptedResult);

                module.Requests.Should().Be(i);
                module.Accepts.Should().Be(i);
            }
        }

        [Test]
        public void Should_increment_only_requests_on_rejected_results()
        {
            for (var i = 1; i <= 10; i++)
            {
                Execute(rejectedResult);

                module.Requests.Should().Be(i);
                module.Accepts.Should().Be(0);
            }
        }

        [Test]
        public void Should_correctly_compute_requests_to_accepts_ratio()
        {
            module.Ratio.Should().Be(0.0);

            Accept(10);

            module.Ratio.Should().Be(1.0);

            Reject(10);

            module.Ratio.Should().Be(2.0);
        }

        [Test]
        public void Should_not_reject_requests_until_minimum_count_is_reached()
        {
            for (var i = 0; i < minimumRequests - 1; i++)
                Execute(rejectedResult).Should().BeSameAs(rejectedResult);
        }

        [Test]
        public void Should_honor_rejection_probability_cap()
        {
            Accept(1);

            Reject(100);

            module.RejectionProbability.Should().Be(probabilityCap);
        }

        [Test]
        public void Should_increase_rejection_probability_as_more_requests_are_rejected()
        {
            Accept(1);
            Reject(1);

            while (module.RejectionProbability < probabilityCap)
            {
                var previous = module.RejectionProbability;

                Reject(1);

                module.RejectionProbability.Should().BeGreaterThan(previous);
            }
        }

        [Test]
        public void Should_gradually_decrease_rejection_probability_to_zero_after_requests_become_accepted_after_big_failure()
        {
            Accept(10);

            while (module.RejectionProbability < probabilityCap)
            {
                Reject(1);
            }

            for (var i = 0; i < 10*1000; i++)
            {
                Accept(1);

                if (module.RejectionProbability <= 0.001)
                    return;
            }

            throw new AssertionFailedException("No requests were rejected in 100 attempts, which was highly expected.");
        }

        [Test]
        public void Should_reject_with_throttled_result_when_rejection_probability_allows()
        {
            options = new AdaptiveThrottlingOptions(Guid.NewGuid().ToString(), 1, minimumRequests, criticalRatio, 1.0);
            module = new AdaptiveThrottlingModule(options);

            Accept(1);

            while (module.RejectionProbability < 0.999)
                Reject(1);

            for (var i = 0; i < 100; i++)
            {
                var requestsBefore = module.Requests;
                var acceptsBefore = module.Accepts;

                var result = Execute(acceptedResult);
                if (result.Status == ClusterResultStatus.Throttled)
                {
                    module.Requests.Should().Be(requestsBefore + 1);
                    module.Accepts.Should().Be(acceptsBefore);
                    return;
                }
            }

            throw new AssertionFailedException("No requests were rejected in 100 attempts, which was highly expected.");
        }

        private void Accept(int count)
        {
            for (var i = 0; i < count; i++)
                Execute(acceptedResult);
        }

        private void Reject(int count)
        {
            for (var i = 0; i < count; i++)
                Execute(rejectedResult);
        }

        private ClusterResult Execute(ClusterResult result)
        {
            return module.ExecuteAsync(context, _ => Task.FromResult(result)).GetAwaiter().GetResult();
        }
    }
}
