namespace Vstk.Logging
{
    public interface ILog
    {
        void Log(LogEvent logEvent);

        bool IsEnabledFor(LogLevel level);
    }
}
