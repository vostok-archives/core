using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;

namespace Vostok.Metrics
{
    public static class IMetricScopeExtensions
    {
        public static IMetricScope WithTag(
            this IMetricScope scope,
            string key,
            string value)
        {
            return new MetricScopeTagEnricher(
                scope,
                new Dictionary<string, string> {{key, value}});
        }

        public static IMetricScope WithTags(
            this IMetricScope scope,
            IReadOnlyDictionary<string, string> tags)
        {
            return new MetricScopeTagEnricher(scope, tags);
        }

        public static ITimer StartTimer(
            this IMetricScope scope)
        {
            return new Timer(scope);
        }
    }
}