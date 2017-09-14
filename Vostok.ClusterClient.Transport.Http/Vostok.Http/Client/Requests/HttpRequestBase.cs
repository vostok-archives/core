using System;
using System.Net;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests
{
	public abstract class HttpRequestBase
	{
		protected HttpRequestBase(HttpMethod method, HttpRequestHeaders headers, IHttpContent body)
		{
			if (body != null)
			{
				Preconditions.EnsureCondition(!(body is PeekContent), "body", "PeekContent is not allowed to be used as request body.");
				Preconditions.EnsureCondition(MethodUtilities.MethodAllowsBody(method), "method", "Method {0} can't be used with request body.", method);
			}
			this.method = method;
			this.headers = headers;
			this.body = body;
		}

		public HttpMethod Method
		{
			get { return method; }
		}

		public HttpRequestHeaders Headers
		{
			get { return headers ?? (headers = new HttpRequestHeaders()); }
		}

		public IHttpContent Body
		{
			get { return body; }
		}

		internal bool HasHeaders()
		{
			return headers != null;
		}

		protected string ToStringInternal(string uri)
		{
			return String.Format("{0} {1} HTTP/{2}", method, uri, HttpVersion.Version11);
		}

		protected readonly HttpMethod method;
		protected readonly IHttpContent body;
		protected HttpRequestHeaders headers;
	}
}