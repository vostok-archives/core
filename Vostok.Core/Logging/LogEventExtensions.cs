using System.Collections.Generic;

namespace Vostok.Logging
{
    public static class LogEventExtensions
    {
        public static LogEvent AddProperties(this LogEvent logEvent, IReadOnlyDictionary<string, object> properties)
        {
            return new LogEvent(
                logEvent.Level,
                logEvent.Exception,
                logEvent.MessageTemplate,
                logEvent.MessageParameters,
                logEvent.Properties.Union(properties));
        }

        public static LogEvent AddProperty(this LogEvent logEvent, string key, object value)
        {
            var properties = new Dictionary<string, object>();
            foreach (var kvp in logEvent.Properties)
            {
                properties.Add(kvp.Key, kvp.Value);
            }
            properties.Add(key, value);
            return new LogEvent(
                logEvent.Level,
                logEvent.Exception,
                logEvent.MessageTemplate,
                logEvent.MessageParameters,
                properties);
        }
        private static IReadOnlyDictionary<string, object> Union(this IReadOnlyDictionary<string, object> first, IReadOnlyDictionary<string, object> second)
        {
            var result = new Dictionary<string, object>();

            foreach (var kvp in first)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in second)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}
