using System;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// Represents a retry strategy with fixed attempts count and a zero delay between attempts.
    /// </summary>
    public class ImmediateRetryStrategy : IRetryStrategy
    {
        public ImmediateRetryStrategy(int attemptsCount)
        {
            AttemptsCount = attemptsCount;
        }

        public int AttemptsCount { get; }

        public TimeSpan GetRetryDelay(int attemptsUsed)
        {
            return TimeSpan.Zero;
        }
    }
}
