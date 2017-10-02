using System.Diagnostics;

namespace Vostok.Metrics
{
    internal class Timer : ITimer
    {
        private readonly IMetricEventWriter writer;
        private readonly Stopwatch sw;

        public Timer(IMetricScope scope)
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