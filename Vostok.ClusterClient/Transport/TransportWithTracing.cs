using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Tracing;
using Vostok.Commons.Extensions.Uri;

namespace Vostok.Clusterclient.Transport
{
    internal class TransportWithTracing : ITransport
    {
        static TransportWithTracing()
        {
            Trace.Configuration.ContextFieldsWhitelist.Add(TracingAnnotationNames.Operation);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotationNames.Operation);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotationNames.Service);
        }

        private readonly ITransport transport;

        public TransportWithTracing(ITransport transport)
        {
            this.transport = transport;
        }

        public async Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            Response response;

            using (var span = Trace.BeginSpan())
            {
                span.SetAnnotation(TracingAnnotationNames.Kind, "http-client");
                span.SetAnnotation(TracingAnnotationNames.Component, "cluster-client");
                span.SetAnnotation(TracingAnnotationNames.HttpUrl, request.Url.ToStringWithoutQuery());
                span.SetAnnotation(TracingAnnotationNames.HttpMethod, request.Method);
                span.SetAnnotation(TracingAnnotationNames.HttpRequestContentLength, request.Content?.Length ?? 0);

                response = await transport.SendAsync(request, timeout, cancellationToken).ConfigureAwait(false);

                span.SetAnnotation(TracingAnnotationNames.HttpCode, (int) response.Code);
                span.SetAnnotation(TracingAnnotationNames.HttpResponseContentLength, response.Content.Length);
            }

            return response;
        }
    }
}
