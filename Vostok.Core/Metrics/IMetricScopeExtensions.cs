using System;
using System.Collections;
using System.Collections.Generic;

namespace Vostok.Metrics
{
    public static class IMetricScopeExtensions
    {
        public static IMetricScope WithTag(
            this IMetricScope scope,
            string key,
            string value)
        {
            throw new NotImplementedException();
        }

        public static IMetricScope WithTags(
            this IMetricScope scope,
            params KeyValuePair<string, string>[] tags)
        {
            throw new NotImplementedException();
        }

        public static ITimer StartTimer(
            this IMetricScope scope)
        {
            throw new NotImplementedException();
        }
    }
}