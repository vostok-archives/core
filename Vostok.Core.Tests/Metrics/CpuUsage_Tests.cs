using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Metrics;

namespace Vostok.Metrics
{
    public class CpuUsage_Tests
    {
        [Test, Ignore("explicit argument does not work in VS + Resharper")]
        public void Cpu()
        {
            Console.WriteLine("Nya");
            var scope = new RootMetricScope(new MetricConfiguration
            {
                Reporter = new TestOutputReporter()
            });
            scope.CpuLoad(1.Seconds());


            var compute = new Action(
                () =>
                {
                    var sw = Stopwatch.StartNew();
                    var x = 1.0;
                    while (sw.Elapsed < 30.Seconds())
                    {
                        x = Math.Sin(Math.Sqrt(x + 10) + 3);
                    }
                    Console.WriteLine(x);
                });
            var threads = Enumerable.Range(0, 10).Select(_ => new Thread(() => compute())).ToArray();
            foreach (var t in threads)
            {
                t.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private class TestOutputReporter : IMetricEventReporter
        {
            public void SendEvent(MetricEvent metricEvent)
            {
                throw new NotImplementedException();
            }

            public void SendMetric(MetricEvent metricEvent)
            {
                foreach (var kvp in metricEvent.Values)
                {
                    Console.WriteLine(kvp.Key + " " + kvp.Value);
                }
            }
        }
    }
}