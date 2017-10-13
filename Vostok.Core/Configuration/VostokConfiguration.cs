using System;
using System.Collections.Generic;
using Vostok.Airlock;
using Vostok.Commons.Collections;
using Vostok.Metrics;

namespace Vostok.Configuration
{
    public static class VostokConfiguration
    {
        public static Func<string> Project { get; set; }

        public static Func<string> Environment { get; set; }

        public static Func<string> Service { get; set; }

        public static IAirlockClient AirlockClient { get; set; }
        
        public static class Tracing
        {
            public static ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

            public static ISet<string> InheritedFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

            public static Func<bool> IsEnabled { get; set; } = () => true;
        }

        public static class Metrics
        {
            public static ISet<string> ContextFieldsWhitelist { get; } = new ConcurrentSet<string>(StringComparer.Ordinal);

            public static IMetricEventReporter Reporter { get; set; } = new AirlockMetricReporter(() => AirlockClient, () => RoutingKey.CreatePrefix(Project(), Environment(), Service()));
        }
    }
}