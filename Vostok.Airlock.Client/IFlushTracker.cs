using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal interface IFlushTracker
    {
        Task WaitForFlushRequest();
        FlushRegistration ResetFlushRegistrationList();
        Task RequestFlush();
    }
}