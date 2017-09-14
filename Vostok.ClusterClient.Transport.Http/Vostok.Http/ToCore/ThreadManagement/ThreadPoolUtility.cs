using System;
using System.Diagnostics;
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

            int minimumWorkers;
            int minimumIocp;
            int maximumWorkers;
            int maximumIocp;

            ThreadPool.GetMinThreads(out minimumWorkers, out minimumIocp);
            ThreadPool.GetMaxThreads(out maximumWorkers, out maximumIocp);

            log.Info("Configured ThreadPool: {0}/{1} workers, {2}/{3} IOCP (min/max).",
                minimumWorkers, maximumWorkers, minimumIocp, maximumIocp);
        }

        public static void LogPoolState(ILog log, bool logOnlyWhenExhausted = false)
        {
            int minWorkerThreads;
            int minIocpThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIocpThreads);

            int maxWorkerThreads;
            int maxIocpThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIocpThreads);

            int availableWorkerThreads;
            int availableIocpThread;
            ThreadPool.GetAvailableThreads(out availableWorkerThreads, out availableIocpThread);

            var usedThreads = maxWorkerThreads - availableWorkerThreads;
            var usedIocpThreads = maxIocpThreads - availableIocpThread;

            var message = string.Format("[ThreadPoolStat] min: {0}, used: {1}, minIocp: {2}, usedIocp: {3}, process: {4}",
                minWorkerThreads,
                usedThreads,
                minIocpThreads,
                usedIocpThreads,
                Process.GetCurrentProcess().Threads.Count);

            if (usedThreads > minWorkerThreads)
            {
                log.Warn(message);
            }
            else if (!logOnlyWhenExhausted)
            {
                log.Info(message);
            }
        }

        public static ThreadPoolState GetPoolState()
        {
            int minWorkerThreads;
            int minIocpThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIocpThreads);

            int maxWorkerThreads;
            int maxIocpThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIocpThreads);

            int availableWorkerThreads;
            int availableIocpThread;
            ThreadPool.GetAvailableThreads(out availableWorkerThreads, out availableIocpThread);

            return new ThreadPoolState(minWorkerThreads, maxWorkerThreads - availableWorkerThreads, minIocpThreads,
                maxIocpThreads - availableIocpThread);
        }

        public const int MaximumThreads = 32767;
    }
}