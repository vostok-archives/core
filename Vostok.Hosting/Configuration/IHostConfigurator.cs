using Vostok.Logging;

namespace Vostok.Hosting.Configuration
{
    public interface IHostConfigurator
    {
        void SetEnvironment(string environment);
        void SetHostLog(ILog log);
    }
}