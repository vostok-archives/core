using System.Diagnostics;

namespace Vostok.Metrics
{
    internal class EventStopwatch : IEventStopwatch
    {
        private readonly IMetricEventWriter writer;
        private readonly Stopwatch sw;

        public EventStopwatch(IMetricScope scope)
        {
            writer = scope.WriteEvent();
            sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            var elapsed = sw.Elapsed;
            writer.SetValue("duration", elapsed.TotalMilliseconds);
            writer.Commit();
        }

        public void SetTag(string key, string value)
        {
            writer.SetTag(key, value);
        }

        public void SetValue(string key, double value)
        {
            writer.SetValue(key, value);
        }
    }
}