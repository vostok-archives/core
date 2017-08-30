using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEvent
    {
        public LogEvent(LogLevel level, Exception exception, string messageTemplate, object[] messageParameters, IReadOnlyDictionary<string, object> properties)
        {
            Level = level;
            MessageTemplate = messageTemplate;
            MessageParameters = messageParameters;
            Exception = exception;
            Properties = properties;
        }

        public LogLevel Level { get; }

        public string MessageTemplate { get; }

        public object[] MessageParameters { get; }

        public Exception Exception { get; }

        public IReadOnlyDictionary<string, object> Properties { get; }
    }
}
