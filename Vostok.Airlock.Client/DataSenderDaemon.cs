using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal class DataSenderDaemon : IDataSenderDaemon
    {
        private const int State_NotStarted = 0;
        private const int State_Started = 1;
        private const int State_Disposed = 2;

        private readonly IDataSender dataSender;
        private readonly AirlockConfig config;

        private int currentState;

        private IterationHandle currentIteration;

        public DataSenderDaemon(IDataSender dataSender, AirlockConfig config)
        {
            this.dataSender = dataSender;
            this.config = config;
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
            if (Interlocked.CompareExchange(ref currentState, 1, 1) == State_Started)
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

                var watch = Stopwatch.StartNew();
                var result = await dataSender.SendAsync().ConfigureAwait(false);
                var sendTime = watch.Elapsed;
                currentIteration.ReportSendFinished();

                AdjustSendPeriod(result, ref sendPeriod);
                currentIteration.ScheduleWakeUp(sendPeriod - sendTime);
                await currentIteration.WaitForNextIteration().ConfigureAwait(false);
            }

            ReportNextIteration(null);
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

            public Task WaitForNextIteration()
            {
                return nextIterationWakeup.Task;
            }

            public Task WaitSendFinished()
            {
                return sendFinished.Task;
            }

            public void WakeUp()
            {
                nextIterationWakeup.TrySetResult(1);
            }

            public void ReportSendFinished()
            {
                sendFinished.TrySetResult(1);
            }

            public async Task ScheduleWakeUp(TimeSpan wakeUpAfter)
            {
                if (wakeUpAfter > TimeSpan.Zero)
                {
                    await Task.Delay(wakeUpAfter).ConfigureAwait(false);
                }
                WakeUp();
            }

            public void ReportIterationFinished(IterationHandle nextIteration)
            {
                iterationFinished.TrySetResult(nextIteration);
            }

            public Task<IterationHandle> WaitIterationFinished()
            {
                return iterationFinished.Task;
            }
        }
    }


}