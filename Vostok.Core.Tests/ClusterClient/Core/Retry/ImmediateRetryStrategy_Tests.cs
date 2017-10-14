using System;
using FluentAssertions;
using Vostok.Clusterclient.Retry;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Retry
{
    public class ImmediateRetryStrategy_Tests
    {
        private readonly ImmediateRetryStrategy strategy;

        public ImmediateRetryStrategy_Tests()
        {
            strategy = new ImmediateRetryStrategy(5);
        }

        [Test]
        public void Should_return_correct_attempts_count()
        {
            strategy.AttemptsCount.Should().Be(5);
        }

        [Test]
        public void Should_return_zero_retry_delay_for_each_attempt()
        {
            for (var i = 1; i <= 5; i++)
            {
                strategy.GetRetryDelay(i).Should().Be(TimeSpan.Zero);
            }
        }
    }
}
