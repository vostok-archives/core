using System;
using Vostok.Commons.Utilities;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// Represents a retry strategy with fixed attempts count and a linearly increasing delay between attempts.
    /// </summary>
    public class LinearBackoffRetryStrategy : IRetryStrategy
    {
        /// <param name="attemptsCount">Maximum attempts count.</param>
        /// <param name="initialRetryDelay">Delay before first and second attempts.</param>
        /// <param name="maximumRetryDelay">Upper bound for delay growth.</param>
        /// <param name="retryDelayIncrement">A value added to delay on each retry except the first one.</param>
        /// <param name="jitter">A maximum relative amount of jitter applied to resulting delays.</param>
        public LinearBackoffRetryStrategy(int attemptsCount, TimeSpan initialRetryDelay, TimeSpan maximumRetryDelay, TimeSpan retryDelayIncrement, double jitter = 0.2)
        {
            AttemptsCount = attemptsCount;
            InitialRetryDelay = initialRetryDelay;
            MaximumRetryDelay = maximumRetryDelay;
            RetryDelayIncrement = retryDelayIncrement;
            Jitter = jitter;
        }

        /// <param name="attemptsCount">Maximum attempts count.</param>
        /// <param name="initialRetryDelay">Delay before first and second attempts.</param>
        /// <param name="maximumRetryDelay">Upper bound for delay growth.</param>
        /// <param name="jitter">A maximum relative amount of jitter applied to resulting delays.</param>
        public LinearBackoffRetryStrategy(int attemptsCount, TimeSpan initialRetryDelay, TimeSpan maximumRetryDelay, double jitter = 0.2)
            : this(attemptsCount, initialRetryDelay, maximumRetryDelay, initialRetryDelay, jitter)
        {
        }

        public int AttemptsCount { get; }

        public TimeSpan InitialRetryDelay { get; }

        public TimeSpan MaximumRetryDelay { get; }

        public TimeSpan RetryDelayIncrement { get; }

        public double Jitter { get; }

        public TimeSpan GetRetryDelay(int attemptsUsed)
        {
            var delay = InitialRetryDelay + RetryDelayIncrement.Multiply(Math.Max(0, attemptsUsed - 1));

            var jitterAmount = delay.Multiply(ThreadSafeRandom.NextDouble() * Jitter);

            if (ThreadSafeRandom.NextDouble() <= 0.5)
                jitterAmount = jitterAmount.Negate();

            return TimeSpanExtensions.Min(MaximumRetryDelay, delay + jitterAmount);
        }
    }
}
