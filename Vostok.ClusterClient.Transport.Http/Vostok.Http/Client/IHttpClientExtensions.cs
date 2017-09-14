using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Response;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	public static class IHttpClientExtensions
	{
		#region IPEndPoint and host+port extensions
		public static Task<HttpResponse> SendAsync(this IHttpClient client, HttpScheme scheme, IPEndPoint endPoint, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.SendAsync(relativeRequest.ToHttpRequest(scheme, endPoint), timeout);
		}

		public static Task<HttpResponse> SendAsync(this IHttpClient client, IPEndPoint endPoint, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.SendAsync(HttpScheme.Http, endPoint, relativeRequest, timeout);
		}

		public static Task<HttpResponse> SendAsync(this IHttpClient client, HttpScheme scheme, string host, int port, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.SendAsync(relativeRequest.ToHttpRequest(scheme, host, port), timeout);
		}

		public static Task<HttpResponse> SendAsync(this IHttpClient client, string host, int port, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.SendAsync(HttpScheme.Http, host, port, relativeRequest, timeout);
		}
        #endregion

        #region Synchronous extensions
        public static HttpResponse Send(this IHttpClient client, HttpRequest request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return client.SendAsync(request, timeout, cancellationToken).Result;
        }

        public static HttpResponse Send(this IHttpClient client, HttpRequest request, TimeSpan timeout)
		{
			return client.SendAsync(request, timeout).Result;
		}

		public static HttpResponse Send(this IHttpClient client, HttpScheme scheme, IPEndPoint endPoint, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.SendAsync(relativeRequest.ToHttpRequest(scheme, endPoint), timeout).Result;
		}

		public static HttpResponse Send(this IHttpClient client, IPEndPoint endPoint, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.Send(HttpScheme.Http, endPoint, relativeRequest, timeout);
		}

		public static HttpResponse Send(this IHttpClient client, HttpScheme scheme, string host, int port, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.SendAsync(relativeRequest.ToHttpRequest(scheme, host, port), timeout).Result;
		}

		public static HttpResponse Send(this IHttpClient client, string host, int port, RelativeHttpRequest relativeRequest, TimeSpan timeout)
		{
			return client.Send(HttpScheme.Http, host, port, relativeRequest, timeout);
		}
		#endregion

		#region Callback-based extensions
		public static void BeginSend(this IHttpClient client, HttpRequest request, TimeSpan timeout, Action<HttpResponse> onSuccess, Action<Exception> onError)
		{
			client.SendAsync(request, timeout).ContinueWith(task => PerformCallback(task, onSuccess, onError));
		}

		public static void BeginSend(this IHttpClient client, HttpScheme scheme, IPEndPoint endPoint, RelativeHttpRequest relativeRequest, TimeSpan timeout, Action<HttpResponse> onSuccess, Action<Exception> onError)
		{
			client.SendAsync(relativeRequest.ToHttpRequest(scheme, endPoint), timeout).ContinueWith(task => PerformCallback(task, onSuccess, onError));
		}

		public static void BeginSend(this IHttpClient client, IPEndPoint endPoint, RelativeHttpRequest relativeRequest, TimeSpan timeout, Action<HttpResponse> onSuccess, Action<Exception> onError)
		{
			client.BeginSend(HttpScheme.Http, endPoint, relativeRequest, timeout, onSuccess, onError);
		}

		public static void BeginSend(this IHttpClient client, HttpScheme scheme, string host, int port, RelativeHttpRequest relativeRequest, TimeSpan timeout, Action<HttpResponse> onSuccess, Action<Exception> onError)
		{
			client.SendAsync(relativeRequest.ToHttpRequest(scheme, host, port), timeout).ContinueWith(task => PerformCallback(task, onSuccess, onError));
		}

		public static void BeginSend(this IHttpClient client, string host, int port, RelativeHttpRequest relativeRequest, TimeSpan timeout, Action<HttpResponse> onSuccess, Action<Exception> onError)
		{
			client.BeginSend(HttpScheme.Http, host, port, relativeRequest, timeout, onSuccess, onError);
		}

		private static void PerformCallback(Task<HttpResponse> task, Action<HttpResponse> onSuccess, Action<Exception> onError)
		{
			try
			{
				onSuccess(task.Result);
			}
			catch (Exception error)
			{
				onError(error);
			}
		}
		#endregion
	}
}