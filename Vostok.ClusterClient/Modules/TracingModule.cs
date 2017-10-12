using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Extensions.Uri;
using Vostok.Tracing;

namespace Vostok.Clusterclient.Modules
{
    internal class TracingModule : IRequestModule
    {
        static TracingModule()
        {
            Trace.Configuration.ContextFieldsWhitelist.Add(TracingAnnotationNames.Operation);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotationNames.Operation);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotationNames.Service);
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
                    span.SetAnnotation(TracingAnnotationNames.Service, serviceName);

                span.SetAnnotation(TracingAnnotationNames.Kind, "cluster-client");
                span.SetAnnotation(TracingAnnotationNames.Component, "cluster-client");
                span.SetAnnotation(TracingAnnotationNames.ClusterStrategy, context.Strategy.ToString());
                span.SetAnnotation(TracingAnnotationNames.HttpUrl, context.Request.Url.ToStringWithoutQuery());
                span.SetAnnotation(TracingAnnotationNames.HttpMethod, context.Request.Method);
                span.SetAnnotation(TracingAnnotationNames.HttpRequestContentLength, context.Request.Content?.Length ?? 0);

                clusterResult = await next(context).ConfigureAwait(false);

                span.SetAnnotation(TracingAnnotationNames.ClusterStatus, clusterResult.Status);
                span.SetAnnotation(TracingAnnotationNames.HttpCode, (int) clusterResult.Response.Code);
                span.SetAnnotation(TracingAnnotationNames.HttpResponseContentLength, clusterResult.Response.Content.Length);
            }

            return clusterResult;
        }
    }
}
