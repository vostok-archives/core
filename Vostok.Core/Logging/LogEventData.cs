using System;
using System.Collections.Generic;

namespace Vostok.Logging
{
    public sealed class LogEventData
    {
        public long Timestamp { get; set; }

        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}