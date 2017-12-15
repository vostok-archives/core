using System.Threading.Tasks;

namespace Vostok.Airlock
{
    internal interface IFlushTracker
    {
        Task WaitForFlushRequest();
        FlushRegistrationList ResetFlushRegistrationList();
        Task RequestFlush();
    }
}