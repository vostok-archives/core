using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal class FlushRegistrationList
    {
        public Task ProcessingRequested => processingRequestedTcs.Task;
        private readonly TaskCompletionSource<byte> processingRequestedTcs;
        public Task ProcessingCompleted => processingCompletedTcs.Task;
        private readonly TaskCompletionSource<byte> processingCompletedTcs;

        public FlushRegistrationList()
        {
            processingRequestedTcs = new TaskCompletionSource<byte>(TaskCreationOptions.RunContinuationsAsynchronously);
            processingCompletedTcs = new TaskCompletionSource<byte>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public void RequestProcessing()
        {
            processingRequestedTcs.TrySetResult(1);
        }

        public void ReportProcessingCompleted()
        {
            processingCompletedTcs.TrySetResult(1);
        }
    }
}