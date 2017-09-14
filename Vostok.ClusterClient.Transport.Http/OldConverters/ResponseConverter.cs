using Vostok.Clusterclient.Model;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Response;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using ByteArrayContent = Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent.ByteArrayContent;

namespace Vostok.ClusterClient.Transport.Http.OldConverters
{
    internal class ResponseConverter : IResponseConverter
    {
        public Response Convert(HttpResponse response)
        {
            return new Response(ConvertCode(response.Code), ConvertBody(response.Body), ConvertHeaders(response.Headers));
        }

        private static ResponseCode ConvertCode(HttpResponseCode code)
        {
            return (ResponseCode) (int) code;
        }

        private static Content ConvertBody(ByteArrayContent body)
        {
            return body.Length == 0 ? Content.Empty : new Content(body.Buffer, body.Offset, (int) body.Length);
        }

        private static Headers ConvertHeaders(HttpResponseHeaders headers)
        {
            if (headers.Count == 0)
                return Headers.Empty;
            
            var convertedHeaders = new Headers(headers.Count);

            foreach (var key in headers.Keys)
            {
                convertedHeaders = convertedHeaders.Set(key, headers[key]);
            }

            return convertedHeaders;
        }
    }
}