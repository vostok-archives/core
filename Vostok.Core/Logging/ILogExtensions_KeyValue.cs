using System;
using System.Collections.Generic;
using System.Text;

namespace Vostok.Logging
{
    public static class ILogExtensions_KeyValue
    {
        public static ILog With<T>(this ILog log)
        {
            return log.With(typeof(T));
        }

        public static ILog With(this ILog log, Type t)
        {
            return log.WithSourceContext(t.FullName);
        }

        public static ILog WithSourceContext(this ILog log, object sourceValue)
        {
            return log.WithKeyValue("SourceContext", sourceValue);
        }

        public static ILog WithKeyValue(this ILog log, string key, object value)
        {
            return new LogWithKeyValue(log, key, value);
        }

        private class LogWithKeyValue : ILog
        {
            private readonly ILog log;
            private readonly string key;
            private readonly object value;

            public LogWithKeyValue(ILog log, string key, object value)
            {
                this.log = log;
                this.key = key;
                this.value = value;
            }

            public void Log(LogEvent logEvent)
            {
                log.Log(logEvent.AddProperty(key, value));
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return log.IsEnabledFor(level);
            }
        }

    }
}
