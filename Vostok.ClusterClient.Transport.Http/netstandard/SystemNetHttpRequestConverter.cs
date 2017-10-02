using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class SystemNetHttpRequestConverter
    {
        private static readonly Dictionary<string, HttpMethod> MethodsMapping =
            RequestMethods.All.ToDictionary(m => m, m => new HttpMethod(m));

        public static HttpRequestMessage Convert(Request request)
        {
            var message = new HttpRequestMessage(MethodsMapping[request.Method], request.Url);

            if (request.Content != null)
            {
                message.Content = new ByteArrayContent(request.Content.Buffer, request.Content.Offset, request.Content.Length);
            }

            foreach (var header in request.Headers ?? Enumerable.Empty<Header>())
            {
                TryAddHeader(header, message);
            }

            return message;
        }

        private static bool TryAddHeader(Header header, HttpRequestMessage message)
        {
            // TODO(iloktionov): message.Content can be null here.
            return SystemNetHttpHeaderUtilities.IsContentHeader(header.Name)
                ? message.Content.Headers.TryAddWithoutValidation(header.Name, header.Value)
                : message.Headers.TryAddWithoutValidation(header.Name, header.Value);
        }
    }
}
