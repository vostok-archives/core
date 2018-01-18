using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies.TimeoutProviders;
using Vostok.ClusterClient.Tests.Helpers;

namespace Vostok.ClusterClient.Tests.Core.Strategies.TimeoutProviders
{
    public class EqualTimeoutsProvider_Tests
    {
        private Request request;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("/foo");
        }

        [Test]
        public void Should_throw_an_error_when_given_negative_division_factor()
        {
            Action action = () => new EqualTimeoutsProvider(-1);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_throw_an_error_when_given_zero_division_factor()
        {
            Action action = () => new EqualTimeoutsProvider(0);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_return_all_remaining_budget_when_current_replica_index_is_equal_or_greater_than_division_factor()
        {
            var provider = new EqualTimeoutsProvider(3);

            var budget = Budget.WithRemaining(5.Seconds());

            provider.GetTimeout(request, budget, 3, 5).Should().Be(5.Seconds());
            provider.GetTimeout(request, budget, 4, 5).Should().Be(5.Seconds());
        }

        [Test]
        public void Should_return_all_remaining_budget_when_division_factor_equals_one()
        {
            var provider = new EqualTimeoutsProvider(1);

            var budget = Budget.WithRemaining(5.Seconds());

            for (var i = 0; i < 5; i++)
            {
                provider.GetTimeout(request, budget, i, 5).Should().Be(5.Seconds());
            }
        }

        [Test]
        public void Should_divide_remaining_budget_correctly_for_first_replicas()
        {
            var provider = new EqualTimeoutsProvider(4);

            var budget = Budget.WithRemaining(24.Seconds());

            provider.GetTimeout(request, budget, 0, 5).Should().Be(6.Seconds());
            provider.GetTimeout(request, budget, 1, 5).Should().Be(8.Seconds());
            provider.GetTimeout(request, budget, 2, 5).Should().Be(12.Seconds());
            provider.GetTimeout(request, budget, 3, 5).Should().Be(24.Seconds());
            provider.GetTimeout(request, budget, 4, 5).Should().Be(24.Seconds());
        }
    }
}
