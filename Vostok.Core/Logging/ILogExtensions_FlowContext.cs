using Vostok.Flow;

namespace Vostok.Logging
{
    public static class ILogExtensions_FlowContext
    {
        public static ILog WithFlowContext(this ILog log)
        {
            return new LogWithFlowContext(log);
        }

        private class LogWithFlowContext : ILog
        {
            private readonly ILog log;

            public LogWithFlowContext(ILog log)
            {
                this.log = log;
            }

            public void Log(LogEvent logEvent)
            {
                foreach (var kvp in Context.Properties.Current)
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
