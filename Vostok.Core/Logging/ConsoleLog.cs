using System;

namespace Vostok.Logging
{
    // TODO(iloktionov): Implement more features (formatting, filtering by level, coloring, etc.).

    public class ConsoleLog : ILog
    {
        public void Log(LogEvent logEvent)
        {
            Console.Out.WriteLine($"{logEvent.Level} {string.Format(logEvent.MessageTemplate, logEvent.MessageParameters)} {logEvent.Exception}");
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return true;
        }
    }
}