using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataSenderDaemon : IDataSenderDaemon
    {
        private const int State_NotStarted = 0;
        private const int State_Started = 1;
        private const int State_Disposed = 2;

        private readonly IDataSender dataSender;
        private readonly AirlockConfig config;
        private readonly ILog log;

        private int currentState;

        private IterationHandle currentIteration;

        public DataSenderDaemon(IDataSender dataSender, AirlockConfig config, ILog log)
        {
            this.dataSender = dataSender;
            this.config = config;
            this.log = log;
            currentIteration = null;
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref currentState, State_Started, State_NotStarted) == State_NotStarted)
            {
                Task.Run(SendingRoutine);
            }
        }

        public async Task FlushAsync()
        {
            if (Interlocked.CompareExchange(ref currentState, 0, 0) == State_Started)
            {
                var iteration = currentIteration;
                if (iteration == null)
                {
                    return;
                }

                iteration.WakeUp();
                var nextIteration = await iteration.WaitIterationFinished();
                if (nextIteration == null)
                {
                    return;
                }
                
                await nextIteration.WaitSendFinished();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref currentState, State_Disposed) != State_Disposed)
            {
                currentIteration?.WakeUp();
            }
        }

        private async Task SendingRoutine()
        {
            var sendPeriod = config.SendPeriod;

            while (Interlocked.CompareExchange(ref currentState, 0, 0) != State_Disposed)
            {
                ReportNextIteration(new IterationHandle());

                var (result, sendTime) = await Send().ConfigureAwait(false);

                AdjustSendPeriod(result, ref sendPeriod);

                currentIteration.ScheduleWakeUp(sendPeriod - sendTime);

                await currentIteration.WaitForNextIteration().ConfigureAwait(false);
            }

            ReportNextIteration(null);
        }

        private async Task<(DataSendResult, TimeSpan)> Send()
        {
            var watch = Stopwatch.StartNew();

            DataSendResult result;
            try
            {
                result = await dataSender.SendAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Warn("Send failed with exception", ex);
                result = DataSendResult.Ok;
            }

            var sendTime = watch.Elapsed;
            currentIteration.ReportSendFinished();
            return (result, sendTime);
        }

        private void ReportNextIteration(IterationHandle handle)
        {
            var previousIteration = currentIteration;
            currentIteration = handle;
            previousIteration?.ReportIterationFinished(handle);
        }

        private void AdjustSendPeriod(DataSendResult result, ref TimeSpan sendPeriod)
        {
            if (result == DataSendResult.Backoff)
            {
                sendPeriod = TimeSpan.FromTicks(Math.Min(sendPeriod.Ticks*2, config.SendPeriodCap.Ticks));
            }
            else
            {
                sendPeriod = config.SendPeriod;
            }
        }

        private class IterationHandle
        {
            private readonly TaskCompletionSource<byte> nextIterationWakeup;
            private readonly TaskCompletionSource<byte> sendFinished;
            private readonly TaskCompletionSource<IterationHandle> iterationFinished;

            public IterationHandle()
            {
                nextIterationWakeup = new TaskCompletionSource<byte>();
                sendFinished = new TaskCompletionSource<byte>();
                iterationFinished = new TaskCompletionSource<IterationHandle>();
            }

            public Task WaitSendFinished()
            {
                return sendFinished.Task;
            }

            public void ReportSendFinished()
            {
                sendFinished.TrySetResult(1);
            }

            public Task WaitForNextIteration()
            {
                return nextIterationWakeup.Task;
            }

            public void WakeUp()
            {
                nextIterationWakeup.TrySetResult(1);
            }

            public async Task ScheduleWakeUp(TimeSpan wakeUpAfter)
            {
                if (wakeUpAfter > TimeSpan.Zero)
                {
                    await Task.Delay(wakeUpAfter).ConfigureAwait(false);
                }
                WakeUp();
            }

            public Task<IterationHandle> WaitIterationFinished()
            {
                return iterationFinished.Task;
            }

            public void ReportIterationFinished(IterationHandle nextIteration)
            {
                iterationFinished.TrySetResult(nextIteration);
            }
        }
    }
}