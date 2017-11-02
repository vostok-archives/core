using System.Threading.Tasks;

namespace Vostok.Hosting
{
    public delegate Task<Task> StartServiceDelegate(IVostokHostingEnvironment hostingEnvironment);
}