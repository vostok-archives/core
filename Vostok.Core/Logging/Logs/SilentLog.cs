namespace Vostok.Logging.Logs
{
    public class SilentLog : ILog
    {
        public void Log(LogEvent logEvent)
        {
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return false;
        }
    }
}
