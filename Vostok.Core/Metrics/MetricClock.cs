using System;
using System.Threading;
using Vostok.Commons.Extensions.UnitConvertions;

namespace Vostok.Metrics
{
    public class MetricClock
    {
        public static readonly TimeSpan DefaultPeriod = 1.Minutes();

        private event Action<DateTimeOffset> Actions;
        private int isRunning;
        private DateTimeOffset aggregationTimestamp;
        private Thread aggregationThread;
        private CancellationTokenSource cts;

        public MetricClock(TimeSpan period)
        {
            Period = period;
            isRunning = 0;
        }

        public TimeSpan Period { get; }

        public void Register(Action<DateTimeOffset> action)
        {
            Actions += action;
        }

        public void Unregister(Action<DateTimeOffset> action)
        {
            Actions -= action;
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref isRunning, 1, 0) != 0)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            aggregationTimestamp = new DateTimeOffset(now.Ticks - now.Ticks%Period.Ticks, TimeSpan.Zero);
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

        private void CollectMetricsLoop()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                WaitForNextAggregation();

                if (Actions != null)
                    foreach (var action in Actions.GetInvocationList())
                    {
                        try
                        {
                            ((Action<DateTimeOffset>)action).Invoke(aggregationTimestamp);
                        }
                        catch (Exception)
                        {
                            //TODO (@ezsilmar) Log here
                        }
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
    }
}