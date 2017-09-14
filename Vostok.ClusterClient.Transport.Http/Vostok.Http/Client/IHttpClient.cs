using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Response;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	public interface IHttpClient
	{
		Task<HttpResponse> SendAsync(HttpRequest request, TimeSpan timeout);
		Task<HttpResponse> SendAsync(HttpRequest request, TimeSpan timeout, CancellationToken cancellationToken);
	}
}