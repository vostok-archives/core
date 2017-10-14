using System;
using System.Collections.Generic;
using Vostok.Airlock;
using Vostok.Configuration;

namespace Vostok.Tracing
{
    internal class DefaultTraceConfiguration : ITraceConfiguration
    {
        public ISet<string> ContextFieldsWhitelist => VostokConfiguration.Tracing.ContextFieldsWhitelist;

        public ISet<string> InheritedFieldsWhitelist => VostokConfiguration.Tracing.InheritedFieldsWhitelist;

        public Func<bool> IsEnabled => VostokConfiguration.Tracing.IsEnabled;

        public Func<string> AirlockRoutingKey => () => RoutingKey.Create(VostokConfiguration.Project(), VostokConfiguration.Environment(), VostokConfiguration.Service(), RoutingKey.TracesSuffix);

        public IAirlockClient AirlockClient => VostokConfiguration.AirlockClient;
    }
}
