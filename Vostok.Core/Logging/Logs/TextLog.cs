using System;

namespace Vostok.Logging.Logs
{
    public class TextLog : ILog
    {
        private readonly Action<string> output;
        private readonly LogLevel minLevel;

        public TextLog(Action<string> output, LogLevel minLevel = LogLevel.Trace)
        {
            this.output = output;
            this.minLevel = minLevel;
        }

        public void Log(LogEvent logEvent)
        {
            if (logEvent.Level < minLevel)
                return;

            output.Invoke(LogEventFormatter.Format(logEvent));
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return level >= minLevel;
        }
    }
}