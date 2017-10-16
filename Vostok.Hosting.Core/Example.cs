using System;
using System.Collections.Generic;
using Vostok.Clusterclient.Topology;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Hosting
{
    public class Example
    {
        public Example()
        {
            VostokConfiguration.Project = () => GetSetting<string>("proj"); // live
            VostokConfiguration.Environment = () => GetSetting<string>("env"); // live
            VostokConfiguration.Service = () => GetSetting<string>("serv"); // live

            VostokConfiguration.Airlock.Log = new SilentLog(); // stale
            VostokConfiguration.Airlock.Parallelism = GetSetting<int>("airlockParalellizm"); // stale
            VostokConfiguration.Airlock.ApiKey = GetSetting<string>("airlockApiKey"); // stale
            VostokConfiguration.Airlock.ClusterProvider = new AdHocClusterProvider(() => GetTopology("airlock")); // live
            VostokConfiguration.Airlock.SendPeriod = GetSetting<TimeSpan>("airlockSendPeriod"); // stale
            // ... the rest airlock stale configuration

            VostokConfiguration.Metrics.ContextFieldsWhitelist.Add("dcId"); // stale

            VostokConfiguration.Tracing.ContextFieldsWhitelist.Add("dcId"); // stale
            VostokConfiguration.Tracing.InheritedFieldsWhitelist.Add("myInheritedField"); // stale
            VostokConfiguration.Tracing.IsEnabled = () => GetSetting<bool>("enableTracing"); // live

            var x = VostokConfiguration.Logging.RoutingKey; // it's not a configuration - use it just to obtain a routingKey valid for logging
        }

        private IList<Uri> GetTopology(string airlock)
        {
            throw new NotImplementedException();
        }

        private T GetSetting<T>(string name)
        {
            throw new System.NotImplementedException();
        }

        private ILog CreateLogFor(string loggerName)
        {
            // create logger from some LogManager
            throw new System.NotImplementedException();
        }
    }
}