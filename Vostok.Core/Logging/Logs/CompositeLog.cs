using System.Linq;

namespace Vostok.Logging.Logs
{
    public class CompositeLog : ILog
    {
        private readonly ILog[] logs;

        public CompositeLog(params ILog[] logs)
        {
            this.logs = logs;
        }

        public void Log(LogEvent logEvent)
        {
            foreach (var log in logs)
                log.Log(logEvent);
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return logs.Any(l => l.IsEnabledFor(level));
        }
    }
}