using System;
using System.Text;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Threading;
using Vostok.Logging.Logs;

namespace TestApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            AirlockSerializerRegistry.Register(new Serializer());

            ThreadPoolUtility.Setup(new SilentLog());

            var config = new AirlockConfig
            {
                ClusterProvider = new FixedClusterProvider(
                    new Uri("http://vostok04:8888/"), 
                    new Uri("http://vostok06:8888/")),
                ApiKey = "UniversalApiKey",
                SendPeriod = 1.Seconds(),
                SendPeriodCap = 30.Seconds(),
            };

            var airlock = new ParallelAirlockClient(config, 8, new ConsoleLog());
            var item = Guid.NewGuid().ToString();

            while (true)
            {
                airlock.Push("iloktionov-test", item);
            }
        }

        private class Serializer : IAirlockSerializer<string>
        {
            public void Serialize(string item, IAirlockSink sink)
            {
                sink.Writer.Write(item, Encoding.UTF8);
            }
        }
    }
}
