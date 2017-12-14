using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataSenderDaemon : IDataSenderDaemon
    {
        private const int stateNotStarted = 0;
        private const int stateStarted = 1;
        private const int stateDisposed = 2;

        private readonly IDataSender dataSender;
        private readonly AirlockConfig config;
        private readonly ILog log;
        private readonly AtomicInt currentState;

        private volatile IterationHandle currentIteration;
        private readonly Guid id = Guid.NewGuid();

        public DataSenderDaemon(IDataSender dataSender, AirlockConfig config, ILog log)
        {
            this.dataSender = dataSender;
            this.config = config;
            this.log = log;

            currentState = new AtomicInt(stateNotStarted);
            currentIteration = null;
        }

        public void Start()
        {
            log.Warn($"{id:N} Start. Started.");
            if (currentState.TrySet(stateStarted, stateNotStarted))
            {
                log.Warn($"{id:N} Start. Passed start check.");
                Task.Run(SendingRoutine);
                log.Warn($"{id:N} Start. Started routine.");
            }
            log.Warn($"{id:N} Start. Finished");
        }

        public async Task FlushAsync()
        {
            log.Warn($"{id:N} Flush. Started");
            if (currentState.Value == stateStarted)
            {
                var iteration = currentIteration;
                if (iteration == null)
                {
                    log.Warn($"{id:N} Flush. Finished. Iteration was null");
                    return;
                }
                
                iteration.WakeUp();
                log.Warn($"{id:N} Flush. Waked up current iteration");
                var nextIteration = await iteration.WaitIterationFinished();
                log.Warn($"{id:N} Flush. Current iteration wait finished.");
                if (nextIteration == null)
                {
                    log.Warn($"{id:N} Flush. Finished. Next iteration was null");
                    return;
                }
                
                await nextIteration.WaitSendFinished();
                log.Warn($"{id:N} Flush. Finished. Next iteration send finished.");
            }
        }

        public void Dispose()
        {
            log.Warn($"{id:N} Dispose. Started.");
            FlushAsync().GetAwaiter().GetResult();
            log.Warn($"{id:N} Dispose. Flush finished.");
            currentState.Value = stateDisposed;
            log.Warn($"{id:N} Dispose. Set state to disposed");
            currentIteration?.WakeUp(); // Вот здесь потрачено - можем прождать целый период отправки.
            log.Warn($"{id:N} Dispose. Finished. Waked up current iteration");
        }

        private async Task SendingRoutine()
        {
            log.Warn($"{id:N} Routine. Started");
            var sendPeriod = config.SendPeriod;

            while (currentState.Value != stateDisposed)
            {
                log.Warn($"{id:N} Routine. Passed dispose check");
                ReportNextIteration(new IterationHandle());
                log.Warn($"{id:N} Routine. Next iteration reported");

                var (result, sendTime) = await SendAsync().ConfigureAwait(false);
                log.Warn($"{id:N} Routine. Send finished");

                AdjustSendPeriod(result, ref sendPeriod);

                currentIteration.ScheduleWakeUp(sendPeriod - sendTime);

                log.Warn($"{id:N} Routine. Going to sleep.");
                await currentIteration.WaitForNextIteration().ConfigureAwait(false);
                log.Warn($"{id:N} Routine. Waked up for next iteration");
            }

            log.Warn($"{id:N} Routine. Failed dispose check.");
            ReportNextIteration(null);
            log.Warn($"{id:N} Routine. Finished. Reported null to current iteration");
        }

        private async Task<(DataSendResult, TimeSpan)> SendAsync()
        {
            var watch = Stopwatch.StartNew();

            DataSendResult result;
            try
            {
                result = await dataSender.SendAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Warn($"{id:N} Send failed with exception", ex);
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

            public void ScheduleWakeUp(TimeSpan wakeUpAfter)
            {
                if (wakeUpAfter > TimeSpan.Zero)
                {
                    Task.Delay(wakeUpAfter).ContinueWith(_ => WakeUp());
                }
                else
                {
                    WakeUp();
                }
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