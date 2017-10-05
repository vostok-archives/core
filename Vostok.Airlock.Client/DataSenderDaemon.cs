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
        private readonly CancellationTokenSource cancellationSource;

        private int currentState;

        public DataSenderDaemon(IDataSender dataSender, AirlockConfig config)
        {
            this.dataSender = dataSender;
            this.config = config;

            cancellationSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (Interlocked.CompareExchange(ref currentState, State_Started, State_NotStarted) == State_NotStarted)
            {
                Task.Run(SendingRoutine);
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref currentState, State_Disposed) != State_Disposed)
            {
                cancellationSource.Cancel();
                cancellationSource.Dispose();
            }
        }

        private async Task SendingRoutine()
        {
            var sendPeriod = config.SendPeriod;

            while (Interlocked.CompareExchange(ref currentState, 0, 0) != State_Disposed)
            {
                var watch = Stopwatch.StartNew();

                var result = await dataSender.SendAsync().ConfigureAwait(false);

                var sendTime = watch.Elapsed;

                AdjustSendPeriod(ref sendPeriod, result);

                if (sendPeriod > sendTime)
                    await Task.Delay(sendPeriod - sendTime, cancellationSource.Token).ConfigureAwait(false);
            }
        }

        private void AdjustSendPeriod(ref TimeSpan sendPeriod, DataSendResult result)
        {
            if (result == DataSendResult.Backoff)
            {
                sendPeriod = TimeSpan.FromTicks(Math.Min(sendPeriod.Ticks * 2, config.SendPeriodCap.Ticks));
            }
            else
            {
                sendPeriod = config.SendPeriod;
            }
        }
    }
}