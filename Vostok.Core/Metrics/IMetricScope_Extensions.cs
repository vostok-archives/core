using System;
using System.Xml.Serialization;

namespace Vostok.Metrics
{
    public static class IMetricScope_Extensions
    {
        public static void CpuLoad(
            this IMetricScope scope,
            TimeSpan period)
        {
            var cpuUsage = new CpuUsageMeter();
            scope.Gauge(period, "cpu", () =>
            {
                var processUsage = cpuUsage.Reset().ProcessUsage;
                return processUsage;
            });
        }

        public static void MemoryUsage(
            this IMetricScope scope,
            TimeSpan period)
        {
        }

        public static void NetworkUsage(
            this IMetricScope scope,
            TimeSpan period)
        {
        }

        public static void DiskUsage(
            this IMetricScope scope,
            TimeSpan period)
        {
        }

        public static void ThreadPool(
            this IMetricScope scope,
            TimeSpan period)
        {
        }

        public static void GC(
            this IMetricScope scope,
            TimeSpan period)
        {
        }

        public static void Uptime(
            this IMetricScope scope,
            TimeSpan period)
        {
            var startTimestamp = DateTimeOffset.UtcNow;
            scope.Gauge(
                period,
                "uptime",
                () => (DateTimeOffset.UtcNow - startTimestamp).TotalMilliseconds);
        }

        public static void SystemMetrics(this IMetricScope scope, TimeSpan period)
        {
            var systemScope = scope.WithTag("type", "system");
            systemScope.CpuLoad(period);
            systemScope.MemoryUsage(period);
            systemScope.DiskUsage(period);
            systemScope.NetworkUsage(period);
            systemScope.ThreadPool(period);
            systemScope.GC(period);
            systemScope.Uptime(period);
        }
    }
}