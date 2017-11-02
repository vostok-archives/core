using System.Threading;
using Vostok.Airlock;
using Vostok.Airlock.Tracing;
using Vostok.Logging;
using Vostok.Metrics;
using Vostok.Tracing;

namespace Vostok.Hosting
{
    public class VostokHostingEnvironment : IVostokHostingEnvironment
    {
        private static readonly AsyncLocal<IVostokHostingEnvironment> hostingEnvironmentContainer = new AsyncLocal<IVostokHostingEnvironment>();

        static VostokHostingEnvironment()
        {
            Trace.Configuration.Reporter = new AirlockTraceReporter(() => Current?.AirlockClient, () => RoutingKey.TryCreate(Current?.Project, Current?.Environment, Current?.Service, RoutingKey.TracesSuffix));
        }

        private readonly CancellationTokenSource shutdownCtc = new CancellationTokenSource();

        public string Project { get; set; }
        public string Environment { get; set; }
        public string Service { get; set; }
        public IAirlockClient AirlockClient { get; set; }
        public IMetricScope MetricScope { get; set; }
        public ILog HostLog { get; set; }
        public CancellationToken ShutdownCancellationToken => shutdownCtc.Token;

        public static IVostokHostingEnvironment Current
        {
            get => hostingEnvironmentContainer.Value;
            set => hostingEnvironmentContainer.Value = value;
        }

        public void RequestShutdown()
        {
            shutdownCtc.Cancel();
        }
    }
}