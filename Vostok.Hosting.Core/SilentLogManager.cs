using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Hosting
{
    public class SilentLogManager : ILogManager
    {
        private readonly ILog log = new SilentLog();

        public ILog GetLog(string loggerName)
        {
            return log;
        }
    }
}