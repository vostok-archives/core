using System;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Hosting
{
    internal class VostokHost : IVostokHost
    {
        private readonly IVostokApplication vostokApplication;
        private Task workTask;

        public VostokHost(IVostokHostingEnvironment hostingEnvironment, IVostokApplication vostokApplication)
        {
            this.vostokApplication = vostokApplication;
            HostingEnvironment = hostingEnvironment;
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
                    HostingEnvironment.Log.Info($"Starting service: {HostingEnvironment.Project}/{HostingEnvironment.Service}");
                    HostingEnvironment.Log.Info($"Environment: {HostingEnvironment.Environment}");
                    
                    try
                    {
                        await vostokApplication.StartAsync(HostingEnvironment).ConfigureAwait(false);
                        HostingEnvironment.ShutdownCancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        HostingEnvironment.Log.Info("Service stopped");
                        return;
                    }
                    catch (Exception e)
                    {
                        HostingEnvironment.Log.Fatal(e, "Failed to start service");
                        startTcs.SetException(e);
                        return;
                    }
                    HostingEnvironment.Log.Info(shutdownMessage ?? "Service started");
                    startTcs.SetResult(0);
                    try
                    {
                        await vostokApplication.WaitForTerminationAsync().ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        HostingEnvironment.Log.Fatal(e, "Service failed");
                        throw;
                    }
                    HostingEnvironment.Log.Info("Service stopped");
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