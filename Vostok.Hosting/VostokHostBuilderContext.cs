using Microsoft.Extensions.Configuration;

namespace Vostok.Hosting
{
    public class VostokHostBuilderContext
    {
        public IVostokHostingEnvironment HostingEnvironment { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}