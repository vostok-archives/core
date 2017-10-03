using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using Vostok.Commons.Collections;

namespace Vostok.Metrics
{
    public class MetricClock
    {
        public TimeSpan Period { get; }
        private readonly ConcurrentBag<Action<DateTimeOffset>> actions;
        
        private int isRunning;
        private DateTimeOffset aggregationTimestamp;
        private Thread aggregationThread;
        private CancellationTokenSource cts;

        public MetricClock(TimeSpan period)
        {
            this.Period = period;
            this.isRunning = 0;
            this.actions = new ConcurrentBag<Action<DateTimeOffset>>();
        }

        public void Register(Action<DateTimeOffset> action)
        {
            actions.Add(action);
        }

        private void CollectMetricsLoop()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                WaitForNextAggregation();

                foreach (var action in actions)
                {
                    //TODO (@ezsilmar) try catch and log here
                    action(aggregationTimestamp);
                }

                aggregationTimestamp += Period;
            }
        }

        private void WaitForNextAggregation()
        {
            var now = DateTimeOffset.UtcNow;
            while (now < aggregationTimestamp)
            {
                var timeToSleep = Max(aggregationTimestamp - now, TimeSpan.Zero);
                timeToSleep = Min(timeToSleep, TimeSpan.FromMilliseconds(50));

                Thread.Sleep(timeToSleep);

                if (cts.Token.IsCancellationRequested)
                {
                    break;
                }

                now = DateTimeOffset.UtcNow;
            }
        }

        private TimeSpan Max(TimeSpan x, TimeSpan y)
        {
            return x > y ? x : y;
        }

        private TimeSpan Min(TimeSpan x, TimeSpan y)
        {
            return x < y ? x : y;
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref isRunning, 1, 0) != 0)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            aggregationTimestamp = new DateTimeOffset(now.Ticks - (now.Ticks % Period.Ticks), TimeSpan.Zero);
            cts = new CancellationTokenSource();
            aggregationThread = new Thread(CollectMetricsLoop) {IsBackground = true};
            aggregationThread.Start();
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref isRunning, 0, 1) != 1)
            {
                return;
            }

            cts.Cancel();
            aggregationThread.Join();
        }
    }
}