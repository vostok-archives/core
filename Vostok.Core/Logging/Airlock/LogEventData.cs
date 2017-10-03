using System;
using System.Collections.Generic;

namespace Vostok.Logging.Airlock
{
    public sealed class LogEventData
    {
        public DateTimeOffset Timestamp { get; set; }

        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}