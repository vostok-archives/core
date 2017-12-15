using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal class FlushTracker : IFlushTracker
    {
        private FlushRegistrationList registrationList;

        public Task RequestFlush()
        {
            var currentRegistrationList = Interlocked.CompareExchange(ref registrationList, null, null);
            currentRegistrationList.RequestProcessing();
            return currentRegistrationList.ProcessingCompleted;
        }

        public Task WaitForFlushRequest()
        {
            return Interlocked.CompareExchange(ref registrationList, null, null).ProcessingRequested;
        }

        public FlushRegistrationList ResetFlushRegistrationList()
        {
            var nextRegistrationList = new FlushRegistrationList();
            var currentRegistrationList = Interlocked.Exchange(ref registrationList, nextRegistrationList);
            return currentRegistrationList;
        }
    }
}