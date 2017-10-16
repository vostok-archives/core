using System;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;
using Vostok.Logging;

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

        public int Parallelism { get; set; }

        public ILog Log { get; set; }

        public IAirlockClient Client => lazyClient.Value;

        private IAirlockClient CreateAirlockClient()
        {
            if (Parallelism <= 1)
                return new AirlockClient(this, Log);
            return new ParallelAirlockClient(this, Parallelism, Log);
        }
    }
}