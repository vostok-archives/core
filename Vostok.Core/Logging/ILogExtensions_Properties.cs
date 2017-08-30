using System.Collections.Generic;

namespace Vostok.Logging
{
    public static class ILogExtensions_Properties
    {
        public static ILog WithProperties(this ILog log, IReadOnlyDictionary<string, object> properties)
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
                log.Log(logEvent.AddProperties(properties));
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return log.IsEnabledFor(level);
            }
        }
    }
}
