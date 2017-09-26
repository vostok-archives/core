using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Flow;

namespace Vostok.Clusterclient.Transport
{
    internal class TransportWithDistributedContext : ITransport
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

        private static Request BuildRequestWithDistributedContext(Request request)
        {
            var distributedContext = Context.SerializeDistributedProperties();
            if (distributedContext == null)
            {
                return request;
            }

            foreach (var pair in distributedContext)
            {
                var key = Encode(MakeHeaderKey(pair.Key));

                if (request.Headers?[key] == null)
                {
                    var value = Encode(pair.Value);
                    request = request.WithHeader(key, value);
                }
            }

            return request;
        }

        private static string MakeHeaderKey(string key)
        {
            return HeaderNames.XDistributedContextPrefix + key;
        }

        private static string Encode(string str)
        {
            return UrlEncodingHelper.UrlPathEncode(str);
        }
    }
}
