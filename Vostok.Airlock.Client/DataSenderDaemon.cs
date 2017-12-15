using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataSenderDaemon : IDisposable
    {
        private readonly IDataSender sender;
        private readonly IFlushTracker flushTracker;
        private readonly AirlockConfig config;
        private readonly ILog log;

        private readonly Task routine;
        private readonly TaskCompletionSource<byte> routineCancellation;

        public DataSenderDaemon(
            IDataSender sender,
            IFlushTracker flushTracker,
            AirlockConfig config,
            ILog log)
        {
            this.sender = sender;
            this.flushTracker = flushTracker;
            this.config = config;
            this.log = log;

            routineCancellation = new TaskCompletionSource<byte>();
            routine = Routine();
        }

        public void Dispose()
        {
            flushTracker.RequestFlush().GetAwaiter().GetResult();
            routineCancellation.TrySetResult(1);
            routine.GetAwaiter().GetResult();
        }

        private async Task Routine()
        {
            var sendPeriod = config.SendPeriod;
            var sw = new Stopwatch();
            FlushRegistrationList flushRegistrationList = null;

            while (!routineCancellation.Task.IsCompleted)
            {
                var flushRequested = flushTracker.WaitForFlushRequest();
                var wakeUpReason = await Task.WhenAny(
                    ScheduleDelay(sendPeriod - sw.Elapsed),
                    flushRequested,
                    routineCancellation.Task);

                if (wakeUpReason == routineCancellation.Task)
                {
                    return;
                }

                if (wakeUpReason == flushRequested)
                {
                    flushRegistrationList = flushTracker.ResetFlushRegistrationList();
                }

                sw.Restart();
                var sendResult = await sender.SendAsync();
                sw.Stop();
                sendPeriod = GetNextSendPeriod(sendResult, sendPeriod);

                flushRegistrationList?.ReportProcessingCompleted();
                flushRegistrationList = null;
            }
        }

        private Task ScheduleDelay(TimeSpan sleepTime)
        {
            if (sleepTime <= TimeSpan.Zero)
            {
                return Task.CompletedTask;
            }

            return Task.Delay(sleepTime);
        }

        private TimeSpan GetNextSendPeriod(DataSendResult result, TimeSpan sendPeriod)
        {
            if (result == DataSendResult.Backoff)
            {
                return TimeSpan.FromTicks(Math.Min(sendPeriod.Ticks * 2, config.SendPeriodCap.Ticks));
            }

            return config.SendPeriod;
        }
    }
}