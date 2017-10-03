using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Tracing;
using Vostok.Commons;

namespace Vostok.Clusterclient.Transport
{
    internal class TransportWithTracing : ITransport
    {
        static TransportWithTracing()
        {
            Trace.Configuration.ContextFieldsWhitelist.Add(TracingAnnotations.OperationName);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotations.OperationName);
            Trace.Configuration.InheritedFieldsWhitelist.Add(TracingAnnotations.ServiceName);
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
                span.SetAnnotation("kind", "http-client");
                span.SetAnnotation("component", "cluster-client");
                span.SetAnnotation("http.url", request.Url.ToStringWithoutQuery());
                span.SetAnnotation("http.method", request.Method);
                if (request.Content != null)
                    span.SetAnnotation("http.requestСontentLength", request.Content.Length);

                response = await transport.SendAsync(request, timeout, cancellationToken).ConfigureAwait(false);

                span.SetAnnotation("http.code", (int)response.Code);
                span.SetAnnotation("http.responseСontentLength", response.Content.Length);
            }

            return response;
        }
    }
}