using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies.TimeoutProviders;
using Vostok.ClusterClient.Tests.Helpers;

namespace Vostok.ClusterClient.Tests.Core.Strategies.TimeoutProviders
{
    public class AdHocTimeoutsProvider_Tests
    {
        private Request request;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("/foo");
        }

        [Test]
        public void Should_throw_an_error_when_given_null_timeout_providers_array()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Action action = () => new AdHocTimeoutsProvider(null);

            action.ShouldThrow<ArgumentNullException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_throw_an_error_when_given_an_empty_timeout_providers_array()
        {
            Action action = () => new AdHocTimeoutsProvider();

            action.ShouldThrow<ArgumentException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_return_all_set_up_timeouts_one_by_one()
        {
            var provider = new AdHocTimeoutsProvider(() => 5.Seconds(), () => 3.Seconds(), () => 10.Seconds());

            provider.GetTimeout(request, Budget.Infinite, 0, 5).Should().Be(5.Seconds());
            provider.GetTimeout(request, Budget.Infinite, 1, 5).Should().Be(3.Seconds());
            provider.GetTimeout(request, Budget.Infinite, 2, 5).Should().Be(10.Seconds());
        }

        [Test]
        public void Should_limit_set_up_timeouts_by_remaining_time_budget()
        {
            var provider = new AdHocTimeoutsProvider(() => 5.Seconds(), () => 3.Seconds(), () => 10.Seconds());
            var budget = Budget.WithRemaining(4.Seconds());

            provider.GetTimeout(request, budget, 0, 5).Should().Be(4.Seconds());
            provider.GetTimeout(request, budget, 1, 5).Should().Be(3.Seconds());
            provider.GetTimeout(request, budget, 2, 5).Should().Be(4.Seconds());
        }

        [Test]
        public void Should_correctly_implement_whole_remaining_budget_tail_behaviour()
        {
            var provider = new AdHocTimeoutsProvider(() => 5.Seconds(), () => 3.Seconds(), () => 10.Seconds());
            var budget = Budget.WithRemaining(15.Seconds());

            provider.GetTimeout(request, budget, 3, 5).Should().Be(15.Seconds());
            provider.GetTimeout(request, budget, 4, 5).Should().Be(15.Seconds());
        }

        [Test]
        public void Should_correctly_implement_last_value_tail_behaviour()
        {
            var provider = new AdHocTimeoutsProvider(TailTimeoutBehaviour.RepeatLastValue, () => 5.Seconds(), () => 3.Seconds(), () => 10.Seconds());
            var budget = Budget.WithRemaining(15.Seconds());

            provider.GetTimeout(request, budget, 3, 5).Should().Be(10.Seconds());
            provider.GetTimeout(request, budget, 4, 5).Should().Be(10.Seconds());
        }

        [Test]
        public void Should_limit_last_value_tail_timeout_by_remaining_budget()
        {
            var provider = new AdHocTimeoutsProvider(TailTimeoutBehaviour.RepeatLastValue, () => 5.Seconds(), () => 3.Seconds(), () => 7.Seconds());
            var budget = Budget.WithRemaining(6.Seconds());

            provider.GetTimeout(request, budget, 3, 5).Should().Be(6.Seconds());
            provider.GetTimeout(request, budget, 4, 5).Should().Be(6.Seconds());
        }

        [Test]
        public void Should_evaluate_the_delegate_each_time()
        {
            var cumulative = 0.Seconds();

            var provider = new AdHocTimeoutsProvider(
                () =>
                {
                    cumulative += 1.Seconds();

                    return cumulative;
                });

            provider.GetTimeout(request, Budget.Infinite, 0, 3).Should().Be(1.Seconds());
            provider.GetTimeout(request, Budget.Infinite, 0, 3).Should().Be(2.Seconds());
            provider.GetTimeout(request, Budget.Infinite, 0, 3).Should().Be(3.Seconds());
            provider.GetTimeout(request, Budget.Infinite, 0, 3).Should().Be(4.Seconds());
        }
    }
}
