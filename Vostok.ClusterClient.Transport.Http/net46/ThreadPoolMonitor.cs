using System;
using Vostok.Commons.Threading;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    internal class ThreadPoolMonitor
    {
        public static readonly ThreadPoolMonitor Instance = new ThreadPoolMonitor();

        private static readonly TimeSpan minReportInterval = TimeSpan.FromSeconds(1);

        private readonly object syncObject;
        private DateTime lastReportTimestamp;

        public ThreadPoolMonitor()
        {
            syncObject = new object();
            lastReportTimestamp = DateTime.MinValue;
        }

        public void ReportAndFixIfNeeded(ILog log)
        {
            var state = ThreadPoolUtility.GetPoolState();
            if (state.UsedWorkerThreads < state.MinWorkerThreads &&
                state.UsedIocpThreads < state.MinIocpThreads)
                return;

            var currentTimestamp = DateTime.UtcNow;

            lock (syncObject)
            {
                if (currentTimestamp - lastReportTimestamp < minReportInterval)
                    return;

                lastReportTimestamp = currentTimestamp;
            }

            log.Warn(
                "Looks like you're kinda low on ThreadPool, buddy. " +
                $"Workers: {state.UsedWorkerThreads}/{state.MinWorkerThreads}/{state.MaxWorkerThreads}, " +
                $"IOCP: {state.UsedIocpThreads}/{state.MinIocpThreads}/{state.MaxIocpThreads} (busy/min/max).");

            var currentMultiplier = Math.Min(state.MinWorkerThreads/Environment.ProcessorCount, state.MinIocpThreads/Environment.ProcessorCount);
            if (currentMultiplier < 128)
            {
                log.Info("I will configure ThreadPool for you, buddy!");
                ThreadPoolUtility.Setup(log);
            }
        }
    }
}
