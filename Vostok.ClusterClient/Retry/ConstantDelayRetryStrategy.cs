using System;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// Represents a retry strategy with fixed attempts count and a constant delay between attempts.
    /// </summary>
    public class ConstantDelayRetryStrategy : IRetryStrategy
    {
        public ConstantDelayRetryStrategy(int attemptsCount, TimeSpan retryDelay)
        {
            AttemptsCount = attemptsCount;
            RetryDelay = retryDelay;
        }

        public int AttemptsCount { get; }

        public TimeSpan RetryDelay { get; }

        public TimeSpan GetRetryDelay(int attemptsUsed)
        {
            return RetryDelay;
        }
    }
}
