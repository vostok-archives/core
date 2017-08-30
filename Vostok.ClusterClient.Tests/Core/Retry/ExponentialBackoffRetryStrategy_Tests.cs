using FluentAssertions;
using Vostok.Clusterclient.Retry;
using Xunit;

namespace Vostok.Clusterclient.Core.Retry
{
    public class ExponentialBackoffRetryStrategy_Tests
    {
        private readonly ExponentialBackoffRetryStrategy strategy;

        public ExponentialBackoffRetryStrategy_Tests()
        {
            strategy = new ExponentialBackoffRetryStrategy(5, 1.Seconds(), 10.Seconds(), jitter: 0.0);
        }

        [Fact]
        public void Should_return_correct_attempts_count()
        {
            strategy.AttemptsCount.Should().Be(5);
        }

        [Fact]
        public void Should_return_initial_delay_after_first_attempt()
        {
            strategy.GetRetryDelay(1).Should().Be(1.Seconds());
        }

        [Fact]
        public void Should_exponentially_increase_delays_for_subsequent_attempts()
        {
            strategy.GetRetryDelay(2).Should().Be(2.Seconds());
            strategy.GetRetryDelay(3).Should().Be(4.Seconds());
            strategy.GetRetryDelay(4).Should().Be(8.Seconds());
        }

        [Fact]
        public void Should_respect_maximum_retry_delay_limit()
        {
            strategy.GetRetryDelay(5).Should().Be(10.Seconds());
        }
    }
}
