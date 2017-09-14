using System;
using System.Threading;
using Vostok.Logging;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.ToCore.ThreadManagement
{
    public static class ThreadPoolUtility
    {
        public static void SetUp(ILog log, int multiplier = 128)
        {
            if (multiplier <= 0)
            {
                log.Warn("ThreadPool. Unable to setup minimum threads with multiplier {0}.", multiplier);
                return;
            }

            var minimumThreads = Math.Min(Environment.ProcessorCount * multiplier, MaximumThreads);

            ThreadPool.SetMaxThreads(MaximumThreads, MaximumThreads);

            ThreadPool.SetMinThreads(minimumThreads, minimumThreads);

            ThreadPool.GetMinThreads(out var minimumWorkers, out var minimumIocp);
            ThreadPool.GetMaxThreads(out var maximumWorkers, out var maximumIocp);

            log.Info("Configured ThreadPool: {0}/{1} workers, {2}/{3} IOCP (min/max).",
                minimumWorkers, maximumWorkers, minimumIocp, maximumIocp);
        }

        public const int MaximumThreads = 32767;
    }
}