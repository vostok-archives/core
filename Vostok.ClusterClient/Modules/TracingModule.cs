using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Strategies;
using Vostok.Tracing;

namespace Vostok.Clusterclient.Modules
{
    internal class TracingModule : IRequestModule
    {
        private readonly IRequestStrategy requestStrategy;

        public TracingModule(IRequestStrategy requestStrategy)
        {
            this.requestStrategy = requestStrategy;
        }

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            ClusterResult clusterResult;
            using (var span = Trace.BeginSpan(null))
            {
                span.SetAnnotation("kind", "cluster-client");
                span.SetAnnotation("component", "cluster-client");
                span.SetAnnotation("cluster.strategy", requestStrategy.GetType().Name);
                span.SetAnnotation("http.url", context.Request.Url.ToString(false));
                span.SetAnnotation("http.url", context.Request.Method);
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