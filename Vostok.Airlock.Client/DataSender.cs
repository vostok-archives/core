using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Synchronization;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataSender : IDataSender
    {
        private readonly IDataBatchesFactory batchesFactory;
        private readonly IRequestSender requestSender;
        private readonly ILog log;
        private readonly AtomicLong lostItemsCounter;

        public DataSender(IDataBatchesFactory batchesFactory, IRequestSender requestSender, ILog log, AtomicLong lostItemsCounter)
        {
            this.batchesFactory = batchesFactory;
            this.requestSender = requestSender;
            this.log = log;
            this.lostItemsCounter = lostItemsCounter;
        }

        public async Task<DataSendResult> SendAsync()
        {
            foreach (var batch in batchesFactory.CreateBatches())
            {
                var watch = Stopwatch.StartNew();

                var result = await requestSender.SendAsync(batch.SerializedMessage).ConfigureAwait(false);

                LogBatchSendResult(batch, result, watch.Elapsed);

                if (result == RequestSendResult.IntermittentFailure)
                    return DataSendResult.Backoff;

                if (result == RequestSendResult.DefinitiveFailure)
                    lostItemsCounter.Add(batch.ItemsCount);

                DiscardSnapshots(batch);
            }

            return DataSendResult.Ok;
        }

        private static void DiscardSnapshots(IDataBatch batch)
        {
            foreach (var slice in batch.ParticipatingSlices)
            {
                // TODO(iloktionov): Fix this bullshit!
                slice.Buffer.DiscardSnapshot();
            }
        }

        private void LogBatchSendResult(IDataBatch batch, RequestSendResult result, TimeSpan elapsed)
        {
            if (result == RequestSendResult.Success)
            {
                log.Info($"Successfully sent batch of size {batch.SerializedMessage.Count.Bytes()} in {elapsed}.");
            }
            else
            {
                log.Warn($"Failed to send batch of size {batch.SerializedMessage.Count.Bytes()} with result '{result}' in {elapsed}.");
            }
        }
    }
}