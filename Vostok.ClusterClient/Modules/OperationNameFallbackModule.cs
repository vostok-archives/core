using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Extensions.Uri;
using Vostok.Flow;
using Vostok.Tracing;

namespace Vostok.Clusterclient.Modules
{
    internal class OperationNameFallbackModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (Context.Properties.Current.ContainsKey(TracingAnnotationNames.Operation))
            {
                return next(context);
            }

            return ExecuteInternal(context, next);
        }

        private static async Task<ClusterResult> ExecuteInternal(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            var operationName = context.Request.Method + " " + context.Request.Url.GetNormalizedPath();
            
            using (Context.Properties.Use(TracingAnnotationNames.Operation, operationName))
            {
                return await next(context).ConfigureAwait(false);
            }
        }
    }
}