using System.Collections.Generic;

namespace Vstk.Metrics
{
    public static class MetricScope_Extensions_Tags
    {
        public static IMetricScope WithTag(this IMetricScope scope, string key, string value)
        {
            return new MetricScopeTagEnricher(scope, new Dictionary<string, string> { { key, value } });
        }

        public static IMetricScope WithTags(this IMetricScope scope, IReadOnlyDictionary<string, string> tags)
        {
            return new MetricScopeTagEnricher(scope, tags);
        }
    }
}