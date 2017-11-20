using System;
using Vostok.Commons.Utilities;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// Represents a retry strategy with fixed attempts count and an exponentially increasing delay between attempts.
    /// </summary>
    public class ExponentialBackoffRetryStrategy : IRetryStrategy
    {
        /// <param name="attemptsCount">Maximum attempts count.</param>
        /// <param name="initialRetryDelay">Delay before first and second attempts.</param>
        /// <param name="maximumRetryDelay">Upper bound for delay growth.</param>
        /// <param name="retryDelayMultiplier">A multiplier applied to delay on each retry except the first one.</param>
        /// <param name="jitter">A maximum relative amount of jitter applied to resulting delays.</param>
        public ExponentialBackoffRetryStrategy(int attemptsCount, TimeSpan initialRetryDelay, TimeSpan maximumRetryDelay, double retryDelayMultiplier = 2, double jitter = 0.2)
        {
            AttemptsCount = attemptsCount;
            InitialRetryDelay = initialRetryDelay;
            MaximumRetryDelay = maximumRetryDelay;
            RetryDelayMultiplier = retryDelayMultiplier;
            Jitter = jitter;
        }

        public int AttemptsCount { get; }

        public TimeSpan InitialRetryDelay { get; }

        public TimeSpan MaximumRetryDelay { get; }

        public double RetryDelayMultiplier { get; }

        public double Jitter { get; }

        public TimeSpan GetRetryDelay(int attemptsUsed)
        {
            var delay = InitialRetryDelay.Multiply(Math.Pow(RetryDelayMultiplier, Math.Max(0, attemptsUsed - 1)));

            var jitterAmount = delay.Multiply(ThreadSafeRandom.NextDouble() * Jitter);

            if (ThreadSafeRandom.NextDouble() <= 0.5)
                jitterAmount = jitterAmount.Negate();

            return TimeSpanExtensions.Min(MaximumRetryDelay, delay + jitterAmount) ;
        }
    }
}
