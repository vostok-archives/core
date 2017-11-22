using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class SystemNetHttpRequestConverter
    {
        private static readonly Dictionary<string, HttpMethod> methodsMapping =
            RequestMethods.All.ToDictionary(m => m, m => new HttpMethod(m));

        private static readonly byte[] emptyByteArray = {};

        public static HttpRequestMessage Convert(Request request)
        {
            var message = new HttpRequestMessage(methodsMapping[request.Method], request.Url);

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

        private static void TryAddHeader(Header header, HttpRequestMessage message)
        {
            if (SystemNetHttpHeaderUtilities.IsContentHeader(header.Name))
            {
                if (message.Content == null)
                {
                    message.Content = new ByteArrayContent(emptyByteArray);
                }

                message.Content.Headers.TryAddWithoutValidation(header.Name, header.Value);
                return;
            }

            message.Headers.TryAddWithoutValidation(header.Name, header.Value);
        }
    }
}
