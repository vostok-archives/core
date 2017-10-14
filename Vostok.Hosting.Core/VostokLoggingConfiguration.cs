using System;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Hosting
{
    public class VostokLoggingConfiguration
    {
        public Func<string, ILog> GetLog { get; set; } = _ => new SilentLog();

        public string RoutingKey => Airlock.RoutingKey.TryCreate(VostokConfiguration.Project(), VostokConfiguration.Environment(), VostokConfiguration.Service(), Airlock.RoutingKey.LogsSuffix);
    }
}