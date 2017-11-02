using Vostok.Airlock;
using Vostok.Logging;

namespace Vostok.Hosting.Configuration
{
    public interface IAirlockConfigurator
    {
        void SetConfig(AirlockConfig airlockConfig);
        void SetParallelizm(int parallelizm);
        void SetLog(ILog log);
    }
}