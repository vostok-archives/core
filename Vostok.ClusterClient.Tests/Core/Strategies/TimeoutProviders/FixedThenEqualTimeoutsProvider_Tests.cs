using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies.TimeoutProviders;
using Vostok.ClusterClient.Tests.Helpers;

namespace Vostok.ClusterClient.Tests.Core.Strategies.TimeoutProviders
{
    public class FixedThenEqualTimeoutsProvider_Tests
    {
        [Test]
        public void Should_return_fixed_timeouts_first_and_then_use_equal_division()
        {
            var provider = new FixedThenEqualTimeoutsProvider(3, 20.Seconds(), 12.Seconds(), 17.Seconds());
            var budget = Budget.WithRemaining(600.Seconds());
            var request = Request.Get("/foo");

            provider.GetTimeout(request, budget, 0, 10).Should().Be(20.Seconds());
            provider.GetTimeout(request, budget, 1, 10).Should().Be(12.Seconds());
            provider.GetTimeout(request, budget, 2, 10).Should().Be(17.Seconds());
            provider.GetTimeout(request, budget, 3, 10).Should().Be(200.Seconds());
            provider.GetTimeout(request, budget, 4, 10).Should().Be(300.Seconds());
            provider.GetTimeout(request, budget, 5, 10).Should().Be(600.Seconds());
            provider.GetTimeout(request, budget, 6, 10).Should().Be(600.Seconds());
        }
    }
}
