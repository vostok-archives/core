using System;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;

namespace Vostok.Hosting
{
    public class VostokAirlockConfiguration : AirlockConfig
    {
        private readonly Lazy<IAirlockClient> lazyClient;

        public VostokAirlockConfiguration()
        {
            ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:8888/"));
            lazyClient = new Lazy<IAirlockClient>(CreateAirlockClient);
        }

        public int Paralellizm { get; set; }

        public IAirlockClient Client => lazyClient.Value;

        private IAirlockClient CreateAirlockClient()
        {
            var log = VostokConfiguration.Logging.GetLog("airlock");
            if (Paralellizm <= 1)
                return new AirlockClient(this, log);
            return new ParallelAirlockClient(this, Paralellizm, log);
        }
    }
}