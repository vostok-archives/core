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
        private readonly CancellationTokenSource cancellationSource;

        private readonly AtomicInt currentState;
        private Task sendingRoutineTask;

        public DataSenderDaemon(IDataSender dataSender, AirlockConfig config, ILog log)
        {
            this.dataSender = dataSender;
            this.config = config;
            this.log = log;

            currentState = new AtomicInt(stateNotStarted);
            cancellationSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (currentState.TrySet(stateStarted, stateNotStarted))
            {
                sendingRoutineTask = Task.Run(SendingRoutine);
            }
        }

        public void Dispose()
        {
            if (currentState.TrySet(stateDisposed, stateStarted))
            {
                cancellationSource.Cancel();
                sendingRoutineTask.Wait();
                cancellationSource.Dispose();
            }
        }

        private async Task SendingRoutine()
        {
            var sendPeriod = config.SendPeriod;

            while (currentState.Value != stateDisposed)
            {
                var (result, sendTime) = await SendAsync().ConfigureAwait(false);

                AdjustSendPeriod(ref sendPeriod, result);

                if (sendPeriod > sendTime)
                {
                    try
                    {
                        await Task.Delay(sendPeriod - sendTime, cancellationSource.Token).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            }

            await SendAsync().ConfigureAwait(false);
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
                log.Warn("Send failed with exception", ex);
                result = DataSendResult.Ok;
            }

            var sendTime = watch.Elapsed;
            return (result, sendTime);
        }

        private void AdjustSendPeriod(ref TimeSpan sendPeriod, DataSendResult result)
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
    }
}