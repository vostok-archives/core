using System.Linq;
using Vostok.Clusterclient.Model;
using Vostok.Flow;

namespace Vostok.Clusterclient.Transforms
{
    public class DistributedContextRequestTransform : IRequestTransform
    {
        public Request Transform(Request request)
        {
            var distributedContext = Context.Properties.BuildDistributedContext();

            if (distributedContext == null || !distributedContext.Any())
            {
                return request;
            }

            var newHeaders = distributedContext.Select(x => new Header(x.Key, x.Value));
            if (request.Headers != null)
            {
                newHeaders = newHeaders.Concat(request.Headers);
            }

            var headersArray = newHeaders.ToArray();
            return new Request(request.Method, request.Url, request.Content, new Headers(headersArray, headersArray.Length));
        }
    }
}