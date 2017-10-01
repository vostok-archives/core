using System;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	internal interface IHttpClient
	{
		Task<Response> SendAsync(Request request, TimeSpan timeout, CancellationToken cancellationToken);
	}
}