using System;
using System.Text;
using System.Threading;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Binary;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging.Logs;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            AirlockSerializerRegistry.Register(new Serializer());

            var config = new AirlockConfig
            {
                ApiKey = "UniversalApiKey",
                ClusterProvider = new FixedClusterProvider(new Uri("http://192.168.0.75:8888/")),
                Log = new ConsoleLog(),
                SendPeriod = 1.Seconds(),
                SendPeriodCap = 30.Seconds(),
            };

            var airlock = new Airlock(config);

            while (true)
            {
                Thread.Sleep(250);

                airlock.Push("iloktionov-test", Guid.NewGuid().ToString());
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
