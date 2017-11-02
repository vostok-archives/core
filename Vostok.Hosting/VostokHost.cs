using System;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Hosting
{
    internal class VostokHost : IVostokHost
    {
        private readonly StartServiceDelegate onStartAsync;
        private Task workTask;

        public VostokHost(IVostokHostingEnvironment hostingEnvironment, StartServiceDelegate onStartAsync)
        {
            HostingEnvironment = hostingEnvironment;
            this.onStartAsync = onStartAsync;
        }

        public IVostokHostingEnvironment HostingEnvironment { get; }

        public async Task StartAsync(string shutdownMessage = null)
        {
            if (workTask != null)
                return;
            var startTcs = new TaskCompletionSource<int>();
            workTask = Task.Run(
                async () =>
                {
                    VostokHostingEnvironment.Current = HostingEnvironment;
                    HostingEnvironment.HostLog.Info($"Starting service: {HostingEnvironment.Project}/{HostingEnvironment.Service}");
                    HostingEnvironment.HostLog.Info($"Environment: {HostingEnvironment.Environment}");
                    Task workerTask;
                    try
                    {
                        workerTask = await onStartAsync(HostingEnvironment).ConfigureAwait(false);
                        HostingEnvironment.ShutdownCancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        HostingEnvironment.HostLog.Info("Service stopped");
                        return;
                    }
                    catch (Exception e)
                    {
                        HostingEnvironment.HostLog.Fatal(e, "Failed to start service");
                        startTcs.SetException(e);
                        return;
                    }
                    HostingEnvironment.HostLog.Info(shutdownMessage ?? "Service started");
                    startTcs.SetResult(0);
                    try
                    {
                        await workerTask.ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        HostingEnvironment.HostLog.Fatal(e, "Service failed");
                        throw;
                    }
                    HostingEnvironment.HostLog.Info("Service stopped");
                });
            await startTcs.Task.ConfigureAwait(false);
        }

        public async Task WaitForTerminationAsync()
        {
            if (workTask != null)
                await workTask.ConfigureAwait(false);
        }
    }
}