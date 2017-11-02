using System.Threading.Tasks;

namespace Vostok.Hosting
{
    public interface IVostokHost
    {
        IVostokHostingEnvironment HostingEnvironment { get; }
        Task StartAsync(string shutdownMessage = null);
        Task WaitForTerminationAsync();
    }
}