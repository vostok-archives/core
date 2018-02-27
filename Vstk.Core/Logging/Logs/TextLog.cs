using System;

namespace Vstk.Logging.Logs
{
    public class TextLog : ILog
    {
        private readonly Action<string> output;

        public TextLog(Action<string> output)
        {
            this.output = output;
        }

        public void Log(LogEvent logEvent)
        {
            output.Invoke(LogEventFormatter.Format(logEvent));
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return true;
        }
    }
}