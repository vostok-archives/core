using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transforms
{
    public class DefaultHeadersTransform : IRequestTransform, IEnumerable<Header>
    {
        public DefaultHeadersTransform([CanBeNull] IEnumerable<Header> defaultHeaders = null)
        {
            if (defaultHeaders == null)
            {
                DefaultHeaders = Headers.Empty;
            }
            else
            {
                var headersArray = defaultHeaders.ToArray();

                DefaultHeaders = new Headers(headersArray, headersArray.Length);
            }
        }

        [NotNull]
        public Headers DefaultHeaders { get; private set; }

        public Request Transform(Request request)
        {
            if (DefaultHeaders.Count == 0)
                return request;

            var newHeaders = DefaultHeaders;

            if (request.Headers?.Count > 0)
            {
                foreach (var header in request.Headers)
                {
                    newHeaders = newHeaders.Set(header.Name, header.Value);
                }
            }

            return new Request(request.Method, request.Url, request.Content, newHeaders);
        }

        public void Add([NotNull] Header header)
        {
            DefaultHeaders = DefaultHeaders.Set(header.Name, header.Value);
        }

        public void Add([NotNull] string name, [NotNull] string value)
        {
            DefaultHeaders = DefaultHeaders.Set(name, value);
        }

        public IEnumerator<Header> GetEnumerator()
        {
            return DefaultHeaders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
