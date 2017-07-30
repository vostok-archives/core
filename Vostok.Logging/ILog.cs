namespace Vostok.Logging
{
    public interface ILog
    {
        void Log(LogEvent logEvent);

        bool IsEnabledFor(LogLevel level);
    }
}