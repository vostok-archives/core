using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;

namespace Vostok.Airlock
{
    internal class DataSender : IDataSender
    {
        private readonly IDataBatchesFactory batchesFactory;
        private readonly IRequestSender requestSender;
        private readonly ILog log;

        public DataSender(IDataBatchesFactory batchesFactory, IRequestSender requestSender, ILog log)
        {
            this.batchesFactory = batchesFactory;
            this.requestSender = requestSender;
            this.log = log;
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

                ReleaseSnapshots(batch);
            }

            return DataSendResult.Ok;
        }

        private static void ReleaseSnapshots(IDataBatch batch)
        {
            foreach (var buffer in batch.ParticipatingBuffers)
            {
                buffer.DiscardSnapshot();
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