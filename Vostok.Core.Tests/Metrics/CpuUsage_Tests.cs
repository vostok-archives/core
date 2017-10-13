using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Vostok.Metrics
{
    public class CpuUsage_Tests
    {
        [Test, Ignore("explicit argument does not work in VS + Resharper")]
        public void Cpu()
        {
            Console.WriteLine("Nya");
            var configuration = Substitute.For<IMetricConfiguration>();
            configuration.Reporter.Returns(new TestOutputReporter());
            var scope = new RootMetricScope(configuration);
            scope.CpuLoad(1.Seconds());


            var compute = new Action(
                () =>
                {
                    var sw = Stopwatch.StartNew();
                    var x = 1.0;
                    while (sw.Elapsed < 10.Seconds())
                        x = Math.Sin(Math.Sqrt(x + 10) + 3);
                    Console.WriteLine(x);
                });
            var threads = Enumerable.Range(0, 10).Select(_ => new Thread(() => compute())).ToArray();
            foreach (var t in threads)
                t.Start();
            foreach (var thread in threads)
                thread.Join();
        }

        private class TestOutputReporter : IMetricEventReporter
        {
            public void SendEvent(MetricEvent metricEvent)
            {
                Console.WriteLine("Event");
                foreach (var kvp in metricEvent.Values)
                    Console.WriteLine($"\t{kvp.Key} {kvp.Value}");
            }

            public void SendMetric(MetricEvent metricEvent)
            {
                Console.WriteLine("Metric");
                foreach (var kvp in metricEvent.Values)
                    Console.WriteLine($"\t{kvp.Key} {kvp.Value}");
            }
        }
    }
}