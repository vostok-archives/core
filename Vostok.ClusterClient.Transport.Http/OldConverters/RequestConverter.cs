using System;
using System.Net.Http.Headers;
using System.Text;
using Vostok.Clusterclient.Model;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using HttpRequestHeaders = Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers.HttpRequestHeaders;

namespace Vostok.ClusterClient.Transport.Http.OldConverters
{
    internal class RequestConverter : IRequestConverter
    {
        public HttpRequest Convert(Request request)
        {
            return new HttpRequest(ConvertMethod(request.Method), request.Url, ConvertHeaders(request.Headers), ConvertBody(request.Content, request.Headers));
        }

        private static HttpMethod ConvertMethod(string method)
        {
            switch (method)
            {
                case RequestMethods.Head:
                    return HttpMethod.HEAD;
                case RequestMethods.Get:
                    return HttpMethod.GET;
                case RequestMethods.Post:
                    return HttpMethod.POST;
                case RequestMethods.Put:
                    return HttpMethod.PUT;
                case RequestMethods.Patch:
                    return HttpMethod.PATCH;
                case RequestMethods.Delete:
                    return HttpMethod.DELETE;
                case RequestMethods.Trace:
                    return HttpMethod.TRACE;
                case RequestMethods.Options:
                    return HttpMethod.OPTIONS;
                default:
                    return HttpMethod.Unknown;
            }
        }

        private static HttpRequestHeaders ConvertHeaders(Headers headers)
        {
            if ((headers == null) || (headers.Count == 0))
                return null;

            var convertedHeaders = new HttpRequestHeaders();

            foreach (var header in headers)
            {
                if (HttpRequestHeaders.IsCorrectCustomHeaderName(header.Name))
                {
                    convertedHeaders.SetCustomHeader(header.Name, header.Value);
                }
                else
                {
                    switch (header.Name)
                    {
                        case HeaderNames.Accept:
                            convertedHeaders.Accept = header.Value;
                            break;

                        case HeaderNames.AcceptCharset:
                            convertedHeaders.AcceptCharset = Encoding.GetEncoding(header.Value);
                            break;

                        case HeaderNames.Authorization:
                            convertedHeaders.Authorization = AuthenticationHeaderValue.Parse(header.Value);
                            break;

                        case HeaderNames.Host:
                            convertedHeaders.Host = header.Value;
                            break;

                        case HeaderNames.IfMatch:
                            convertedHeaders.IfMatch = EntityTagHeaderValue.Parse(header.Value); 
                            break;

                        case HeaderNames.IfModifiedSince:
                            convertedHeaders.IfModifiedSince = DateTime.Parse(header.Value).ToUniversalTime();
                            break;

                        case HeaderNames.Range:
                            convertedHeaders.Range = RangeHeaderValue.Parse(header.Value);
                            break;

                        case HeaderNames.Referer:
                            convertedHeaders.Referer = header.Value;
                            break;

                        case HeaderNames.UserAgent:
                            convertedHeaders.UserAgent = header.Value;
                            break;
                    }
                }
            }

            return convertedHeaders;
        }

        private static ByteArrayContent ConvertBody(Content body, Headers headers)
        {
            var contentTypeHeader = headers?.ContentType;
            var contentRangeHeader = headers?.ContentRange;

            if (body == null && contentTypeHeader == null && contentRangeHeader == null)
                return null;

            var content = body == null 
                ? new ByteArrayContent(new byte[0])
                : new ByteArrayContent(body.Buffer, body.Offset, body.Length);

            if (contentTypeHeader != null)
            {
                content.ContentType = ContentType.Parse(contentTypeHeader);
            }

            if (contentRangeHeader != null)
            {
                content.ContentRange = ContentRangeHeaderValue.Parse(contentRangeHeader);
            }

            return content;
        }
    }
}
