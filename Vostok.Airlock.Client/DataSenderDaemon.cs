using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataSenderDaemon : IDisposable
    {
        private readonly IDataSender sender;
        private readonly AirlockConfig config;
        private readonly ILog log;

        private readonly Task routine;
        private readonly TaskCompletionSource<byte> routineCancellation;

        public DataSenderDaemon(
            IDataSender sender,
            AirlockConfig config,
            ILog log)
        {
            this.sender = sender;
            this.config = config;
            this.log = log;

            routineCancellation = new TaskCompletionSource<byte>();
            routine = Routine();
        }

        /// <inheritdoc />
        /// <summary>
        /// @ezsilmar Be careful, this method is not thread-safe
        /// </summary>
        public void Dispose()
        {
            routineCancellation.TrySetResult(1);
            routine.GetAwaiter().GetResult();
            SendAsync().GetAwaiter().GetResult();
        }

        private async Task Routine()
        {
            var sendPeriod = config.SendPeriod;
            var lastSendElapsed = TimeSpan.Zero;

            while (!routineCancellation.Task.IsCompleted)
            {
                var wakeUpReason = await Task.WhenAny(
                    ScheduleDelay(sendPeriod - lastSendElapsed),
                    routineCancellation.Task).ConfigureAwait(false);

                if (wakeUpReason == routineCancellation.Task)
                {
                    return;
                }

                var sw = Stopwatch.StartNew();
                var sendResult = await SendAsync().ConfigureAwait(false);
                lastSendElapsed = sw.Elapsed;
                sendPeriod = GetNextSendPeriod(sendResult, sendPeriod);
            }
        }

        private async Task<DataSendResult> SendAsync()
        {
            try
            {
                return await sender.SendAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Warn("Send failed with exception", ex);
                return DataSendResult.Ok;
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