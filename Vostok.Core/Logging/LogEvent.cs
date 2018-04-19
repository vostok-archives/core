using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEvent
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public LogEvent(LogLevel level, Exception exception, string messageTemplate, object[] messageParameters)
        {
            Level = level;
            MessageTemplate = messageTemplate;
            MessageParameters = messageParameters;
            Exception = exception;
        }

        public LogLevel Level { get; }

        public string MessageTemplate { get; }

        public object[] MessageParameters { get; }

        public Exception Exception { get; }

        public IReadOnlyDictionary<string, object> Properties => properties;

        public void AddPropertyIfAbsent(string name, object value)
        {
            if (properties.ContainsKey(name))
                return;

            properties.Add(name, value ?? "null");
        }
    }
}