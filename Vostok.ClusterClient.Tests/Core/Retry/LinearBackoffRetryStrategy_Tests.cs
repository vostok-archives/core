using FluentAssertions;
using Vostok.Clusterclient.Retry;
using Xunit;

namespace Vostok.Clusterclient.Core.Retry
{
    public class LinearBackoffRetryStrategy_Tests
    {
        private readonly LinearBackoffRetryStrategy strategy;

        public LinearBackoffRetryStrategy_Tests()
        {
            strategy = new LinearBackoffRetryStrategy(5, 1.Seconds(), 4.Seconds(), 1.Seconds(), 0.0);
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
        public void Should_linearly_increase_delays_for_subsequent_attempts()
        {
            strategy.GetRetryDelay(2).Should().Be(2.Seconds());
            strategy.GetRetryDelay(3).Should().Be(3.Seconds());
        }

        [Fact]
        public void Should_respect_maximum_retry_delay_limit()
        {
            strategy.GetRetryDelay(4).Should().Be(4.Seconds());
            strategy.GetRetryDelay(5).Should().Be(4.Seconds());
        }
    }
}
