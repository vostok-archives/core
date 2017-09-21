using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEventData
    {
        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}