using System;
using System.Diagnostics;
using System.Threading;

namespace Vostok.Metrics
{
    public class CpuUsageMeter
    {
        private readonly Stopwatch sw;
        private long prevCpuTime;

        public CpuUsageMeter()
        {
            sw = Stopwatch.StartNew();
            prevCpuTime = 0;
        }

        public CpuUsageStats Reset()
        {
            try
            {

                var systemTimeDelta = GetSystemTimeDelta();
                var cpuDelta = GetCpuDelta();

                var processUsage = systemTimeDelta == 0 ? 0 : (double) cpuDelta/systemTimeDelta;
                processUsage /= Environment.ProcessorCount;

                return new CpuUsageStats
                {
                    ProcessUsage = processUsage
                };
            }
            catch (Exception)
            {
                return new CpuUsageStats
                {
                    ProcessUsage = 0
                };
            }
        }

        private long GetCpuDelta()
        {
            var processTime = Process.GetCurrentProcess().TotalProcessorTime.Ticks;
            return processTime - Interlocked.Exchange(ref prevCpuTime, processTime);
        }

        private long GetSystemTimeDelta()
        {
            var systemTimeDelta = sw.Elapsed.Ticks;
            sw.Restart();
            return systemTimeDelta;
        }
    }
}