using Vostok.Logging;

namespace Vostok.Hosting
{
    public interface ILogManager
    {
        ILog GetLog(string loggerName);
    }
}