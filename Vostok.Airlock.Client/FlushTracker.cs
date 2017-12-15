using System.Threading;
using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal class FlushTracker : IFlushTracker
    {
        private FlushRegistration registration;

        public FlushTracker()
        {
            registration = new FlushRegistration();
        }

        public Task RequestFlush()
        {
            var currentRegistration = Interlocked.CompareExchange(ref registration, null, null);
            currentRegistration.RequestProcessing();
            return currentRegistration.ProcessingCompleted;
        }

        public Task WaitForFlushRequest()
        {
            return Interlocked.CompareExchange(ref registration, null, null).ProcessingRequested;
        }

        public FlushRegistration ResetFlushRegistrationList()
        {
            var nextRegistration = new FlushRegistration();
            var currentRegistration = Interlocked.Exchange(ref registration, nextRegistration);
            return currentRegistration;
        }
    }
}