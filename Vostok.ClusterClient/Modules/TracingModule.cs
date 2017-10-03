using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Commons;
using Vostok.Tracing;

namespace Vostok.Clusterclient.Modules
{
    internal class TracingModule : IRequestModule
    {
        static TracingModule()
        {
            Trace.Configuration.ContextFieldsWhitelist.Add(TracingAnnotations.OperationName);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotations.OperationName);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotations.ServiceName);
        }

        private readonly string serviceName;

        public TracingModule(string serviceName)
        {
            this.serviceName = serviceName;
        }

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            ClusterResult clusterResult;

            using (var span = Trace.BeginSpan())
            {
                if (!string.IsNullOrEmpty(serviceName))
                    span.SetAnnotation("serviceName", serviceName);

                span.SetAnnotation("kind", "cluster-client");
                span.SetAnnotation("component", "cluster-client");
                span.SetAnnotation("cluster.strategy", context.Strategy.ToString());
                span.SetAnnotation("http.url", context.Request.Url.ToStringWithoutQuery());
                span.SetAnnotation("http.method", context.Request.Method);
                span.SetAnnotation("http.requestСontentLength", context.Request.Content?.Length ?? 0);

                clusterResult = await next(context).ConfigureAwait(false);

                span.SetAnnotation("cluster.status", clusterResult.Status);
                span.SetAnnotation("http.code", (int) clusterResult.Response.Code);
                span.SetAnnotation("http.responseСontentLength", clusterResult.Response.Content.Length);
            }

            return clusterResult;
        }
    }
}
