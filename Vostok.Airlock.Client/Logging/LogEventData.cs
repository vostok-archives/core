using System;
using System.Collections.Generic;
using System.Reflection;
using Vostok.Logging;

namespace Vostok.Airlock.Logging
{
    public sealed class LogEventData
    {
        public LogEventData(Exception ex = null)
        {
            if (ex == null)
                return;
            Message = ex.Message;
            Exceptions = new List<LogEventException>();
            var currentEx = ex;
            while (currentEx != null)
            {
                Exceptions.Add(new LogEventException(currentEx));
                currentEx = currentEx.InnerException;
            }
            if (!(ex is ReflectionTypeLoadException typeLoadException))
                return;
            foreach (var loaderException in typeLoadException.LoaderExceptions)
                Exceptions.Add(new LogEventException(loaderException));
        }

        public DateTimeOffset Timestamp { get; set; }

        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public List<LogEventException> Exceptions;

        public IDictionary<string, string> Properties { get; set; }
    }
}