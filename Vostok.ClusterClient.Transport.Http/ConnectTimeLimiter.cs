using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Transport.Http
{
    // ReSharper disable PossibleInvalidOperationException

    internal class ConnectTimeLimiter
    {
        private readonly VostokHttpTransportSettings settings;
        private readonly ILog log;

        public ConnectTimeLimiter(VostokHttpTransportSettings settings, ILog log)
        {
            this.settings = settings;
            this.log = log;
        }

        public Task<HttpActionStatus> LimitConnectTime(Task<HttpActionStatus> mainTask, Request request, HttpWebRequestState state)
        {
            if (settings.ConnectionTimeout == null)
                return mainTask;

            if (!ConnectTimeoutHelper.CanCheckSocket)
                return mainTask;

            if (state.TimeRemaining < settings.ConnectionTimeout.Value)
                return mainTask;

            if (request.Url.IsLoopback)
                return mainTask;

            if (ConnectTimeoutHelper.IsSocketConnected(state.Request, log))
                return mainTask;

            return LimitConnectTimeInternal(mainTask, request, state);
        }

        private async Task<HttpActionStatus> LimitConnectTimeInternal(Task<HttpActionStatus> mainTask, Request request, HttpWebRequestState state)
        {
            using (var timeoutCancellation = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(mainTask, Task.Delay(settings.ConnectionTimeout.Value, timeoutCancellation.Token)).ConfigureAwait(false);
                if (completedTask is Task<HttpActionStatus> taskWithResult)
                {
                    timeoutCancellation.Cancel();
                    return taskWithResult.GetAwaiter().GetResult();
                }

                if (!ConnectTimeoutHelper.IsSocketConnected(state.Request, log))
                {
                    state.CancelRequestAttempt();
                    LogConnectionFailure(request, new WebException($"Connection attempt timed out. Timeout = {settings.ConnectionTimeout.Value}.", WebExceptionStatus.ConnectFailure), state.ConnectionAttempt);
                    return HttpActionStatus.ConnectionFailure;
                }

                return await mainTask.ConfigureAwait(false);
            }
        }

        private void LogConnectionFailure(Request request, WebException error, int attempt)
        {
            log.Error($"Connection failure. Target = {request.Url.Authority}. Attempt = {attempt}/{settings.ConnectionAttempts}. Status = {error.Status}.", error.InnerException ?? error);
        }
    }
}