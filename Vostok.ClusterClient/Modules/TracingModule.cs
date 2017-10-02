using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Tracing;
using Vostok.Commons;

namespace Vostok.Clusterclient.Modules
{
    internal class TracingModule : IRequestModule
    {
        public TracingModule(string serviceName)
        {
        }

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            ClusterResult clusterResult;
            var operationName = context.OperationName ?? context.Request.Url.Normalize();

            using (var span = Trace.BeginSpan(operationName))
            {
                span.SetAnnotation("kind", "cluster-client");
                span.SetAnnotation("component", "cluster-client");
                span.SetAnnotation("cluster.strategy", context.Strategy.GetType().Name);
                span.SetAnnotation("http.url", context.Request.Url.ToString(false));
                span.SetAnnotation("http.method", context.Request.Method);
                if (context.Request.Content != null)
                    span.SetAnnotation("http.requestСontentLength", context.Request.Content.Length);

                clusterResult = await next(context).ConfigureAwait(false);

                span.SetAnnotation("cluster.status", clusterResult.Status);
                span.SetAnnotation("http.code", (int) clusterResult.Response.Code);
                span.SetAnnotation("http.responseСontentLength", clusterResult.Response.Content.Length);
            }

            return clusterResult;
        }
    }
}