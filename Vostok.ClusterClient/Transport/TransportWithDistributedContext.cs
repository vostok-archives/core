using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Flow;

namespace Vostok.Clusterclient.Transport
{
    public class TransportWithDistributedContext : ITransport
    {
        private readonly ITransport transport;
        public TransportWithDistributedContext(ITransport transport)
        {
            this.transport = transport;
        }

        public Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var newRequest = BuildRequestWithDistributedContext(request);
            return transport.SendAsync(newRequest, timeout, cancellationToken);
        }

        private Request BuildRequestWithDistributedContext(Request request)
        {
            var distributedContext = Context.SerializeDistributedProperties();

            if (distributedContext == null || !distributedContext.Any())
            {
                return request;
            }

            var newHeaders = distributedContext.Select(x => new Header(Encode(MakeHadearKey(x.Key)), Encode(x.Value)));
            if (request.Headers != null)
            {
                newHeaders = newHeaders.Concat(request.Headers);
            }

            var headersArray = newHeaders.ToArray();
            return new Request(request.Method, request.Url, request.Content, new Headers(headersArray, headersArray.Length));
        }

        private static string MakeHadearKey(string key)
        {
            return "distributed_context/" + key;
        }

        private static string Encode(string str)
        {
            return Uri.EscapeUriString(str);
        }
    }
}