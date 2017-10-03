using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Model;

namespace Vostok.Clusterclient.Modules
{
    internal class RequestPriorityApplicationModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (context.Priority.HasValue)
                RequestPriorityContext.Current = context.Priority;

            return next(context);
        }
    }
}