using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEventData
    {
        public DateTimeOffset Timestamp { get; set; }

        public string LogLevel { get; set; }

        public string MessageTemplate { get; set; }

        public string Exception { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}