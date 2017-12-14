using System;
using System.Diagnostics;
using System.Threading;
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

        //@ezsilmar
        // IterationHandle is needed for async flush support
        // This variable is set only in SendingRoutine thread and its value is read concurrently
        private IterationHandle currentIteration;
        private IterationHandle CurrentIteration
        {
            get => Interlocked.CompareExchange(ref currentIteration, null, null);
            set => Interlocked.Exchange(ref currentIteration, value);
        }

        private readonly Guid id = Guid.NewGuid();

        public DataSenderDaemon(IDataSender dataSender, AirlockConfig config, ILog log)
        {
            this.dataSender = dataSender;
            this.config = config;
            this.log = log;

            currentState = new AtomicInt(stateNotStarted);
            CurrentIteration = null;
        }

        public void Start()
        {
            if (currentState.TrySet(stateStarted, stateNotStarted))
            {
                //@ezsilmar
                //Make sure that current iteration is initialized synchronously here
                //Otherwise we violate dispose invariant: the state is 'stateStarted' but CurrentIteration is null
#pragma warning disable 4014
                SendingRoutine();
#pragma warning restore 4014
            }
        }

        public async Task FlushAsync()
        {
            if (currentState.Value == stateStarted)
            {
                var iteration = CurrentIteration;
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
            FlushAsync().GetAwaiter().GetResult();
            currentState.Value = stateDisposed;
            var iteration = CurrentIteration;
            if (iteration == null)
            {
                return;
            }
            iteration.WakeUp();

            //@ezsilmar
            // Need to check next iteration too,
            // because IterationFinished is set after while loop check in SendRoutine
            var nextIteration = iteration.WaitIterationFinished().GetAwaiter().GetResult();
            if (nextIteration == null)
            {
                return;
            }
            nextIteration.WakeUp();

            nextIteration.WaitIterationFinished().GetAwaiter().GetResult();
        }

        private async Task SendingRoutine()
        {
            var sendPeriod = config.SendPeriod;

            while (currentState.Value != stateDisposed)
            {
                ReportNextIteration(new IterationHandle());

                var (result, sendTime) = await SendAsync().ConfigureAwait(false);

                AdjustSendPeriod(result, ref sendPeriod);

                CurrentIteration.ScheduleWakeUp(sendPeriod - sendTime);

                await CurrentIteration.WaitForNextIteration().ConfigureAwait(false);
            }

            ReportNextIteration(null);
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
            CurrentIteration.ReportSendFinished();
            return (result, sendTime);
        }

        private void ReportNextIteration(IterationHandle handle)
        {
            var previousIteration = CurrentIteration;
            CurrentIteration = handle;
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