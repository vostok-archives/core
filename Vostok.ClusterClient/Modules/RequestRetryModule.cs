using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Retry;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    internal class RequestRetryModule : IRequestModule
    {
        private readonly IRetryPolicy retryPolicy;
        private readonly IRetryStrategy retryStrategy;

        public RequestRetryModule(IRetryPolicy retryPolicy, IRetryStrategy retryStrategy)
        {
            this.retryPolicy = retryPolicy;
            this.retryStrategy = retryStrategy;
        }

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            var attemptsUsed = 0;

            while (true)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var result = await next(context).ConfigureAwait(false);
                if (result.Status != ClusterResultStatus.ReplicasExhausted)
                    return result;

                if (context.Budget.HasExpired)
                    return result;

                if (++attemptsUsed >= retryStrategy.AttemptsCount)
                    return result;

                if (!retryPolicy.NeedToRetry(result.ReplicaResults))
                    return result;

                var retryDelay = retryStrategy.GetRetryDelay(attemptsUsed);
                if (retryDelay >= context.Budget.Remaining)
                    return result;

                context.Log.Info($"All replicas exhausted. Will retry after {retryDelay.ToPrettyString()}. Attempts used: {attemptsUsed}/{retryStrategy.AttemptsCount}.");

                if (retryDelay > TimeSpan.Zero)
                {
                    await Task.Delay(retryDelay, context.CancellationToken).ConfigureAwait(false);
                }

                (context as RequestContext)?.ResetReplicaResults();
            }
        }
    }
}
