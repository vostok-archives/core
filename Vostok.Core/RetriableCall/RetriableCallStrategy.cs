using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.RetriableCall
{
    public interface IRetriableCallStrategy
    {
        Task CallAsync(Func<Task> action, Func<Exception, bool> isExceptionRetriable, ILog log);
        Task<T> CallAsync<T>(Func<Task<T>> action, Func<Exception, bool> isExceptionRetriable, ILog log);
    }

    internal class ExceptionComparer : IComparer<Exception>
    {
        public int Compare(Exception e1, Exception e2)
        {
            var cmp = string.CompareOrdinal(e1.Message, e2.Message);
            return cmp != 0 ? cmp : string.Compare(e1.StackTrace, e2.StackTrace, StringComparison.Ordinal);
        }
    }

    public class RetriableCallStrategy : IRetriableCallStrategy
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

        public async Task<T> CallAsync<T>(Func<Task<T>> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            var res = default(T);
            await CallAsync(async () =>
            {
                res = await action();
            }, isExceptionRetriable, log);
            return res;
        }

        public T Call<T>(Func<T> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            var res = default(T);
            Call(() =>
            {
                res = action();
            }, isExceptionRetriable, log);
            return res;
        }

        public async Task CallAsync(Func<Task> action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            var exceptions = new SortedSet<Exception>(new ExceptionComparer());

            var delay = minDelay;
            var message = "";
            for (var failedTries = 0; failedTries < retriesBeforeStop; failedTries++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception ex)
                {
                    if (ExceptionFinder.FindException(ex, isExceptionRetriable) == null)
                        throw;
                    message = ex.Message;
                    if (failedTries < retriesBeforeStop - 1)
                    {
                        if (failedTries > 0)
                            delay = IncreaseDelay(delay);
                        log.Warn($"Try #{failedTries + 1} failed, retry after {delay.Milliseconds}ms: {ex}");
                        await Task.Delay(delay);
                    }
                    if (!exceptions.Contains(ex))
                        exceptions.Add(ex);
                }
            }
            throw new AggregateException(message, exceptions);
        }

        public void Call(Action action, Func<Exception, bool> isExceptionRetriable, ILog log)
        {
            var exceptions = new SortedSet<Exception>(new ExceptionComparer());

            var delay = minDelay;
            var message = "";
            for (var failedTries = 0; failedTries < retriesBeforeStop; failedTries++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    if (ExceptionFinder.FindException(ex, isExceptionRetriable) == null)
                        throw;
                    message = ex.Message;
                    if (failedTries < retriesBeforeStop - 1)
                    {
                        if (failedTries > 0)
                            delay = IncreaseDelay(delay);
                        log.Warn($"Try #{failedTries + 1} failed, retry after {delay.Milliseconds}ms: {ex}");
                        System.Threading.Thread.Sleep(delay);
                    }
                    if (!exceptions.Contains(ex))
                        exceptions.Add(ex);
                }
            }
            throw new AggregateException(message, exceptions);
        }

        private TimeSpan IncreaseDelay(TimeSpan delay)
        {
            var multiplier = minDelayMultiplier + ThreadSafeRandom.NextDouble() * (maxDelayMultiplier - minDelayMultiplier);
            var increasedDelay = delay.Multiply(multiplier);
            return TimeSpanExtensions.Min(TimeSpanExtensions.Max(minDelay, increasedDelay), maxDelay);
        }

    }

}