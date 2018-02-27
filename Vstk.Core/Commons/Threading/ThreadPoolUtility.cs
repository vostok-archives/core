using System;
using System.Threading;
using Vstk.Logging;

namespace Vstk.Commons.Threading
{
    public static class ThreadPoolUtility
    {
        public const int MaximumThreads = 32767;

        public static void Setup(ILog log, int multiplier = 128)
        {
            if (multiplier <= 0)
            {
                log.Warn($"ThreadPool. Unable to setup minimum threads with multiplier {multiplier}.");
                return;
            }

            var minimumThreads = Math.Min(Environment.ProcessorCount*multiplier, MaximumThreads);

            ThreadPool.SetMaxThreads(MaximumThreads, MaximumThreads);
            ThreadPool.SetMinThreads(minimumThreads, minimumThreads);
            ThreadPool.GetMinThreads(out var minimumWorkers, out var minimumIocp);
            ThreadPool.GetMaxThreads(out var maximumWorkers, out var maximumIocp);

            log.Info(
                "Configured ThreadPool: {0}/{1} workers, {2}/{3} IOCP (min/max).",
                minimumWorkers,
                maximumWorkers,
                minimumIocp,
                maximumIocp);
        }

        public static ThreadPoolState GetPoolState()
        {
            ThreadPool.GetMinThreads(out var minWorkerThreads, out var minIocpThreads);
            ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxIocpThreads);
            ThreadPool.GetAvailableThreads(out var availableWorkerThreads, out var availableIocpThreads);

            return new ThreadPoolState
            {
                MinWorkerThreads = minWorkerThreads,
                MinIocpThreads = minIocpThreads,

                MaxWorkerThreads = maxWorkerThreads,
                MaxIocpThreads = maxIocpThreads,

                UsedWorkerThreads = maxWorkerThreads - availableWorkerThreads,
                UsedIocpThreads = maxIocpThreads - availableIocpThreads
            };
        }
    }
}
