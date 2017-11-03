using System.Threading;
using Microsoft.Extensions.Configuration;
using Vostok.Airlock;
using Vostok.Logging;
using Vostok.Metrics;

namespace Vostok.Hosting
{
    public interface IVostokHostingEnvironment
    {
        string Project { get; set; }
        string Environment { get; set; }
        string Service { get; set; }
        IAirlockClient AirlockClient { get; set; }
        IMetricScope MetricScope { get; set; }
        ILog HostLog { get; set; }
        CancellationToken ShutdownCancellationToken { get; }
        IConfiguration Configuration { get; set; }
        void RequestShutdown();
    }
}