using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Modules
{
    internal class RequestPriorityApplicationModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            // TODO(iloktionov): implement

            return next(context);
        }
    }
}