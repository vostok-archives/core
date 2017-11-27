using Vostok.Airlock;
using Vostok.Logging;

namespace Vostok.Hosting.Configuration
{
    public interface IAirlockConfigurator
    {
        void SetConfig(AirlockConfig airlockConfig);
        void SetParallelism(int parallelism);
        void SetLog(ILog log);
    }
}