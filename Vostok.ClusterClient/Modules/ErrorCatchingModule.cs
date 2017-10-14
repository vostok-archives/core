using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    internal class ErrorCatchingModule : IRequestModule
    {
        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            try
            {
                return await next(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return ClusterResult.Canceled(context.Request);
            }
            catch (Exception error)
            {
                context.Log.Error("Unexpected failure during request execution.", error);
                return ClusterResult.UnexpectedException(context.Request);
            }
        }
    }
}
