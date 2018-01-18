using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Airlock.Logging;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Airlock.Client.Tests
{
    [Ignore("Explicit attribute does not work in VS + Resharper")]
    public class Integration_Tests
    {
        private readonly ConsoleLog log = new ConsoleLog();

        [Test]
        [Repeat(50)]
        public void PushLogEventsToAirlock()
        {
            var routingKey = RoutingKey.Create("vostok", "ci", "core", RoutingKey.LogsSuffix);
            var events = GenerateLogEvens(count: 10);
            PushToAirlock(routingKey, events, e => e.Timestamp);
        }

        private static LogEventData[] GenerateLogEvens(int count)
        {
            var utcNow = DateTimeOffset.UtcNow;
            return Enumerable.Range(0, count)
                             .Select(i => new LogEventData
                             {
                                 Message = "Testing AirlockClient" + i,
                                 Level = LogLevel.Debug,
                                 Timestamp = utcNow.AddSeconds(-i*10)
                             }).ToArray();
        }

        private void PushToAirlock<T>(string routingKey, T[] events, Func<T, DateTimeOffset> getTimestamp)
        {
            log.Debug($"Pushing {events.Length} events to airlock");
            var sw = Stopwatch.StartNew();
            ParallelAirlockClient airlockClient;
            using (airlockClient = CreateAirlockClient())
            {
                foreach (var @event in events)
                    airlockClient.Push(routingKey, @event, getTimestamp(@event));
            }
            log.Debug($"SentItemsCount: {airlockClient.SentItemsCount}, LostItemsCount: {airlockClient.LostItemsCount}, Elapsed: {sw.Elapsed}");
            airlockClient.LostItemsCount.Should().Be(0);
            airlockClient.SentItemsCount.Should().Be(events.Length);
        }

        private ParallelAirlockClient CreateAirlockClient()
        {
            var airlockConfig = new AirlockConfig
            {
                ApiKey = "UniversalApiKey",
                ClusterProvider = new FixedClusterProvider(new Uri("http://vostok.dev.kontur.ru:6306")),
                SendPeriod = TimeSpan.FromSeconds(2),
                SendPeriodCap = TimeSpan.FromMinutes(5),
                RequestTimeout = TimeSpan.FromSeconds(30),
                MaximumRecordSize = 1.Kilobytes(),
                MaximumBatchSizeToSend = 300.Megabytes(),
                MaximumMemoryConsumption = 300.Megabytes(),
                InitialPooledBufferSize = 10.Megabytes(),
                InitialPooledBuffersCount = 10,
                EnableTracing = false
            };
            //return new AirlockClient(airlockConfig, log.FilterByLevel(LogLevel.Warn));
            return new ParallelAirlockClient(airlockConfig, 10, log.FilterByLevel(LogLevel.Warn));
        }
    }
}