using System.Threading.Tasks;

namespace Vostok.Hosting
{
    public interface IVostokApplication
    {
        Task StartAsync(IVostokHostingEnvironment hostingEnvironment);
        Task WaitForTerminationAsync();
    }
}