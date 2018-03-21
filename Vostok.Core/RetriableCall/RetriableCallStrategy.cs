using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vstk.Commons.Extensions.UnitConvertions;
using Vstk.Commons.Utilities;
using Vstk.Logging;

namespace Vstk.RetriableCall
{
    public class RetriableCallStrategy
    {
        private readonly int retriesBeforeStop;
        private const double minDelayMultiplier = 1.7;
        private const double maxDelayMultiplier = 2.5;
        private readonly TimeSpan minDelay;
        private readonly TimeSpan maxDelay;

        public RetriableCallStrategy()
            : this(5, 500.Milliseconds(), 10.Seconds())
        {
        }

        public RetriableCallStrategy(int retriesBeforeStop, TimeSpan minDelay, TimeSpan maxDelay)
        {
            this.retriesBeforeStop = retriesBeforeStop;
            if (this.retriesBeforeStop <= 0)
                this.retriesBeforeStop = 1;
            if (minDelay > maxDelay)
                throw new ArgumentOutOfRangeException(nameof(maxDelay), "should not be less than " + nameof(minDelay));
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
        }

        public async Task CallAsync(Func<Task> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            await CallAsync(
                    async () =>
                    {
                        await action().ConfigureAwait(false);
                        return 0;
                    },
                    isExceptionRetriable,
                    log)
                .ConfigureAwait(false);
        }

        public void Call(Action action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            Call(
                () =>
                {
                    action();
                    return 0;
                },
                isExceptionRetriable,
                log);
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
                    if (!ex.HasException(isExceptionRetriable))
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
                    if (!ex.HasException(isExceptionRetriable))
                        throw;
                    delay = ProcessExceptionAndGetDelay(ex, exceptions, tryNumber, delay, log);
                    if (delay != TimeSpan.Zero)
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
                log.Warn($"Try #{tryNumber + 1} failed, retry after {delay.TotalMilliseconds}ms", ex);
            }
            if (!exceptions.Contains(ex))
                exceptions.Add(ex);
            return delay;
        }

        private TimeSpan IncreaseDelay(TimeSpan delay)
        {
            var multiplier = minDelayMultiplier + ThreadSafeRandom.NextDouble()*(maxDelayMultiplier - minDelayMultiplier);
            var increasedDelay = delay.Multiply(multiplier);
            return TimeSpanExtensions.Min(TimeSpanExtensions.Max(minDelay, increasedDelay), maxDelay);
        }
    }
}