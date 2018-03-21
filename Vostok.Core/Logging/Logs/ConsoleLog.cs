using System;
using System.Collections.Generic;

namespace Vostok.Logging.Logs
{
    public class ConsoleLog : ILog
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> levelToColor = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Trace, ConsoleColor.Gray},
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        private readonly object syncLock = new object();

        public void Log(LogEvent logEvent)
        {
            lock (syncLock)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = levelToColor[logEvent.Level];
                Console.Out.Write(LogEventFormatter.Format(logEvent));
                Console.ForegroundColor = oldColor;
            }
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return true;
        }
    }
}