using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.RetriableCall
{
    public class RetriableCallStrategy
    {
        private readonly int retriesBeforeStop;
        private const double minDelayMultiplier = 1.7;
        private const double maxDelayMultiplier = 2.5;
        private readonly TimeSpan minDelay;
        private readonly TimeSpan maxDelay;

        public RetriableCallStrategy(int retriesBeforeStop = 5, int minDelayMs = 500, int maxDelayMs = 10000)
        {
            this.retriesBeforeStop = retriesBeforeStop;
            if (this.retriesBeforeStop <= 0)
                this.retriesBeforeStop = 1;
            if (minDelayMs < 0)
                throw new ArgumentOutOfRangeException(nameof(minDelayMs), "should not be less than 0");
            if (maxDelayMs < 0)
                throw new ArgumentOutOfRangeException(nameof(maxDelayMs), "should not be less than 0");
            if (minDelayMs > maxDelayMs)
                throw new ArgumentOutOfRangeException(nameof(maxDelayMs), "should not be less than minDelayMs");
            minDelay = TimeSpan.FromMilliseconds(minDelayMs);
            maxDelay = TimeSpan.FromMilliseconds(maxDelayMs);
        }

        public async Task CallAsync(Func<Task> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            await CallAsync(async () =>
            {
                await action().ConfigureAwait(false);
                return 0;
            }, isExceptionRetriable, log).ConfigureAwait(false);
        }

        public void Call(Action action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            Call(() =>
            {
                action();
                return 0;
            }, isExceptionRetriable, log);
        }

        public async Task<T> CallAsync<T>(Func<Task<T>> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            var exceptions = new SortedSet<Exception>(new ExceptionComparer());

            var delay = minDelay;
            for (var tryNumber = 0; tryNumber < retriesBeforeStop; tryNumber++)
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ExceptionFinder.FindException(ex, isExceptionRetriable) == null)
                        throw;
                    delay = ProcessExceptionAndGetDelay(ex, exceptions, tryNumber, delay, log);
                    if (delay != TimeSpan.Zero)
                        await Task.Delay(delay).ConfigureAwait(false);
                }
            }
            throw new AggregateException(exceptions.LastOrDefault()?.Message, exceptions);
        }

        public T Call<T>(Func<T> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            var exceptions = new SortedSet<Exception>(new ExceptionComparer());

            var delay = minDelay;
            for (var tryNumber = 0; tryNumber < retriesBeforeStop; tryNumber++)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    if (ExceptionFinder.FindException(ex, isExceptionRetriable) == null)
                        throw;
                    delay = ProcessExceptionAndGetDelay(ex, exceptions, tryNumber, delay, log);
                    if (delay!=TimeSpan.Zero)
                        System.Threading.Thread.Sleep(delay);
                }
            }
            throw new AggregateException(exceptions.LastOrDefault()?.Message, exceptions);
        }

        private TimeSpan ProcessExceptionAndGetDelay(Exception ex, ISet<Exception> exceptions, int tryNumber, TimeSpan delay, ILog log)
        {
            if (tryNumber >= retriesBeforeStop - 1)
                delay = TimeSpan.Zero;
            else
            {
                if (tryNumber > 0)
                    delay = IncreaseDelay(delay);
                log.Warn($"Try #{tryNumber + 1} failed, retry after {delay.Milliseconds}ms", ex);
            }
            if (!exceptions.Contains(ex))
                exceptions.Add(ex);
            return delay;
        }

        private TimeSpan IncreaseDelay(TimeSpan delay)
        {
            var multiplier = minDelayMultiplier + ThreadSafeRandom.NextDouble() * (maxDelayMultiplier - minDelayMultiplier);
            var increasedDelay = delay.Multiply(multiplier);
            return TimeSpanExtensions.Min(TimeSpanExtensions.Max(minDelay, increasedDelay), maxDelay);
        }

    }

}