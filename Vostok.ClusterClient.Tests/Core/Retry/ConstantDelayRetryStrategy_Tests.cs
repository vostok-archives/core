using FluentAssertions;
using Vostok.Clusterclient.Retry;
using Xunit;

namespace Vostok.Clusterclient.Core.Retry
{
    public class ConstantDelayRetryStrategy_Tests
    {
        private readonly ConstantDelayRetryStrategy strategy;

        public ConstantDelayRetryStrategy_Tests()
        {
            strategy = new ConstantDelayRetryStrategy(3, 1.Seconds());
        }

        [Fact]
        public void Should_return_correct_attempts_count()
        {
            strategy.AttemptsCount.Should().Be(3);
        }

        [Fact]
        public void Should_return_same_retry_delay_for_each_attempt()
        {
            strategy.GetRetryDelay(1).Should().Be(1.Seconds());
            strategy.GetRetryDelay(2).Should().Be(1.Seconds());
            strategy.GetRetryDelay(3).Should().Be(1.Seconds());
        }
    }
}
