using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public static class ILogExtensions
    {
        public static ILog ForContext<T>(this ILog log)
        {
            return log.ForContext(typeof(T));
        }

        public static ILog ForContext<T>(this ILog log, T _)
        {
            return log.ForContext(typeof(T));
        }

        public static ILog ForContext(this ILog log, Type source)
        {
            return log.ForContext("SourceContext", source.FullName);
        }

        public static ILog ForContext(this ILog log, string name, object value)
        {
            return new LogWithProperties(log, new Dictionary<string, object> {{name, value}});
        }

        public static ILog ForContext(this ILog log, IReadOnlyDictionary<string, object> properties)
        {
            return new LogWithProperties(log, properties);
        }

        private class LogWithProperties : ILog
        {
            private readonly ILog log;
            private readonly IReadOnlyDictionary<string, object> properties;

            public LogWithProperties(ILog log, IReadOnlyDictionary<string, object> properties)
            {
                this.log = log;
                this.properties = properties;
            }

            public void Log(LogEvent logEvent)
            {
                foreach (var kvp in properties)
                {
                    logEvent.AddPropertyIfAbsent(kvp.Key, kvp.Value);
                }

                log.Log(logEvent);
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return log.IsEnabledFor(level);
            }
        }
    }
}