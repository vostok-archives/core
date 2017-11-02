using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Hosting
{
    public static class IVostokHost_Extensions
    {
        public static void Run(this IVostokHost host)
        {
            host.RunAsync().GetAwaiter().GetResult();
        }

        public static async Task RunAsync(this IVostokHost host, CancellationToken token = default(CancellationToken))
        {
            if (token.CanBeCanceled)
                await host.RunAsync(token, null);
            else
            {
                using (var done = new ManualResetEventSlim(false))
                using (var cts = new CancellationTokenSource())
                {
                    AttachUnexpectedExceptionLogging(host);
                    AttachCtrlcSigtermShutdown(host, cts, done);
                    await host.RunAsync(cts.Token, "Service started. Press Ctrl+C to shut down.");
                    done.Set();
                }
            }
        }

        private static async Task RunAsync(this IVostokHost host, CancellationToken token, string shutdownMessage)
        {
            token.Register(() => host.HostingEnvironment.RequestShutdown());
            await host.StartAsync(shutdownMessage);
            await host.WaitForTerminationAsync();
        }

        private static void AttachUnexpectedExceptionLogging(IVostokHost host)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                host.HostingEnvironment.HostLog.Fatal((Exception) args.ExceptionObject, "Unexpected exception was occured");
            };
        }

        private static void AttachCtrlcSigtermShutdown(IVostokHost host, CancellationTokenSource cts, ManualResetEventSlim done)
        {
            void ShutdownAndWait(string shutdownMessage)
            {
                if (!cts.IsCancellationRequested)
                {
                    host.HostingEnvironment.HostLog.Info(shutdownMessage);
                    try
                    {
                        cts.Cancel();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
                done.Wait();
            }
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => ShutdownAndWait("Termination signal was received -> terminating...");
            Console.CancelKeyPress += (sender, args) =>
            {
                ShutdownAndWait("Ctrl+C was pressed -> terminating...");
                args.Cancel = true;
            };
        }
    }
}