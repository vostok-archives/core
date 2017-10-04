using System;

namespace Vostok.Logging
{
    public static class ILogExtensions_Filters
    {
        public static ILog FilterByLevel(this ILog log, LogLevel minLevel = LogLevel.Debug)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            return new LogWithLevel(log, minLevel);
        }

        public static ILog FilterByProperty<T>(this ILog log, string propertyName, Func<T, bool> propertyFilter, LogLevel minLevel = LogLevel.Debug)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (propertyFilter == null)
                throw new ArgumentNullException(nameof(propertyFilter));

            return new LogWithPropertyFilter<T>(log, propertyName, propertyFilter, minLevel);
        }

        private class LogWithLevel : ILog
        {
            private readonly ILog log;
            private readonly LogLevel minLevel;

            public LogWithLevel(ILog log, LogLevel minLevel)
            {
                this.log = log;
                this.minLevel = minLevel;
            }

            public void Log(LogEvent logEvent)
            {
                if (logEvent.Level < minLevel)
                    return;

                log.Log(logEvent);
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return level >= minLevel && log.IsEnabledFor(level);
            }
        }

        private class LogWithPropertyFilter<TProperty> : ILog
        {
            private readonly ILog log;
            private readonly string propertyName;
            private readonly Func<TProperty, bool> propertyFilter;
            private readonly LogLevel minLevel;

            public LogWithPropertyFilter(ILog log, string propertyName, Func<TProperty, bool> propertyFilter, LogLevel minLevel)
            {
                this.log = log;
                this.propertyName = propertyName;
                this.propertyFilter = propertyFilter;
                this.minLevel = minLevel;
            }

            public void Log(LogEvent logEvent)
            {
                if (logEvent.Properties.TryGetValue(propertyName, out var propertyValue)
                    && propertyValue is TProperty property
                    && propertyFilter.Invoke(property)
                    && logEvent.Level < minLevel)
                    return;

                log.Log(logEvent);
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return log.IsEnabledFor(level);
            }
        }
    }
}