using System;
using System.Collections.Generic;
using System.Linq;
using Vstk.Logging.Logs;

namespace Vstk.Logging
{
    public static class CompositeLog
    {
        public static ILog Create(params ILog[] logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));
            List<ILog> composition = null;
            ILog result = null;
            foreach (var log in logs)
            {
                if (log != null && !(log is SilentLog))
                {
                    if (result == null)
                        result = log;
                    else 
                    {
                        if (composition == null)
                            composition = new List<ILog>{result};
                        composition.Add(log);
                    }
                }
            }
            if (composition != null)
                return new CompositeLogImpl(composition.ToArray());
            return result ?? new SilentLog();
        }

        private class CompositeLogImpl : ILog
        {
            private readonly ILog[] logs;

            public CompositeLogImpl(ILog[] logs)
            {
                this.logs = logs;
            }

            public void Log(LogEvent logEvent)
            {
                foreach (var log in logs)
                    log.Log(logEvent);
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return logs.Any(l => l.IsEnabledFor(level));
            }
        }
    }
}