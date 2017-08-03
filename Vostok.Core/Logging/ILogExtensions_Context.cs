using Vostok.Flow;

namespace Vostok.Logging
{
    public static class ILogExtensions_Context
    {
        public static ILog WithContext(this ILog log)
        {
            return new LogWithContext(log);
        }

        private class LogWithContext : ILog
        {
            private readonly ILog log;

            public LogWithContext(ILog log)
            {
                this.log = log;
            }

            public void Log(LogEvent logEvent)
            {
                log.Log(logEvent.AddProperties(Context.Properties.Current));
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return log.IsEnabledFor(level);
            }
        }
    }
}