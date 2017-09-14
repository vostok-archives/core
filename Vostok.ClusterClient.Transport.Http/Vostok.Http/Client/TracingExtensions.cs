using System;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Response;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;
using Vostok.Logging;
using Vostok.Tracing;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
    internal static class TracingExtensions
    {
        public static HttpRequest TraceWith(this HttpRequest request, ISpanBuilder spanBuilder, ILog log)
        {
            try
            {
                spanBuilder.SetBeginTimestamp(TODO);

                spanBuilder.SetAnnotation(Annotation.RequestUrl, request.AbsoluteUri.AbsolutePath);
                spanBuilder.SetAnnotation(Annotation.RequestHost, request.AbsoluteUri.Host);
                spanBuilder.SetAnnotation(Annotation.RequestMethod, request.Method.ToString());
                spanBuilder.SetAnnotation(Annotation.SourceId, HttpClientIdentity.Get());
                spanBuilder.SetAnnotation(Annotation.SourceHost, HttpClientHostname.Get());

                if (request.Body != null)
                    spanBuilder.SetAnnotation(Annotation.RequestBodyLength, request.Body.Length);

                if (spanBuilder.CanRecordClientDetails)
                {
                    spanBuilder.RecordClientRequestDetails(MethodUtilities.GetString(request.Method), request.AbsoluteUri, (int)(request.Body?.Length ?? 0));
                }
            }
            catch (Exception error)
            {
                log.Error(error);
            }

            return request;
        }

        public static HttpResponse TraceWith(this HttpResponse response, ISpanBuilder spanBuilder, ILog log)
        {
            try
            {
                spanBuilder.SetEndTimestamp(TODO);

                spanBuilder.SetAnnotation(Annotation.ResponseCode, (int)response.Code);

                if (response.Body.Length > 0)
                    spanBuilder.SetAnnotation(Annotation.ResponseBodyLength, response.Body.Length);

                if (spanBuilder.CanRecordClientDetails)
                {
                    spanBuilder.RecordClientResponseDetails((int)response.Code, (int)response.Body.Length);
                }
            }
            catch (Exception error)
            {
                log.Error(error);
            }

            return response;
        }
    }
}