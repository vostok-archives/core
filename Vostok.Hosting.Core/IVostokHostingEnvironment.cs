using System.Threading;
using Microsoft.Extensions.Configuration;
using Vostok.Airlock;
using Vostok.Logging;
using Vostok.Metrics;

namespace Vostok.Hosting
{
    public interface IVostokHostingEnvironment
    {
        string Project { get; }
        string Environment { get; }
        string Service { get; }
        IConfiguration Configuration { get; }
        IAirlockClient AirlockClient { get; }
        IMetricScope MetricScope { get; }
        ILog HostLog { get; }
        CancellationToken ShutdownCancellationToken { get; }
        void RequestShutdown();
    }
}