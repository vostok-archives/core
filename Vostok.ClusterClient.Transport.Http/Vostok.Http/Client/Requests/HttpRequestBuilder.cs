using System;
using System.Net;
using System.Net.Http.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;
using HttpRequestHeaders = Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers.HttpRequestHeaders;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests
{
	public class HttpRequestBuilder
	{
		private HttpRequestBuilder(HttpMethod method, Uri baseUri, string query)
		{
			this.method = method;
			this.baseUri = baseUri;
			this.query = query;
		}

		public static HttpRequestBuilder StartNew(HttpMethod method, Uri baseUri)
		{
			Preconditions.EnsureCondition(baseUri.IsAbsoluteUri, "baseUri", "Base Uri must be absolute!");
			return new HttpRequestBuilder(method, baseUri, baseUri.Query);
		}

		public static HttpRequestBuilder StartNew(HttpMethod method, string baseUri)
		{
			return StartNew(method, new Uri(baseUri, UriKind.Absolute));
		}

		public static HttpRequestBuilder StartNew(HttpMethod method, HttpScheme scheme, string host, int port, string path = null)
		{
			Uri baseUri = new UriBuilder(SchemeUtilities.ToLowercaseString(scheme), host, port, path ?? String.Empty).Uri;
			return new HttpRequestBuilder(method, baseUri, null);
		}

		public static HttpRequestBuilder StartNew(HttpMethod method, HttpScheme scheme, IPEndPoint endPoint, string path = null)
		{
			return StartNew(method, scheme, endPoint.Address.ToString(), endPoint.Port, path);
		}

		public HttpRequest Build()
		{
			return new HttpRequest(method, BuildRequestUri(), headers, body);
		}

	    public HttpRequestBuilder AddPathSegment(string segment, bool encode = true)
	    {
	        if (segment.Contains("/"))
                throw new ArgumentException("A path segment cannot contain '/' character.");

	        if (encode)
	            segment = UrlEncodingUtility.UrlEncode(segment);

            baseUri = new Uri(baseUri, baseUri.AbsolutePath.TrimEnd('/') + '/' + segment);

	        return this;
	    }

		public HttpRequestBuilder AddToQuery(string key, string value)
		{
			string encodedKey = UrlEncodingUtility.UrlEncode(key, EncodingFactory.GetDefault());
			string encodedValue = UrlEncodingUtility.UrlEncode(value, EncodingFactory.GetDefault());
			if (String.IsNullOrEmpty(query))
				query = String.Format("?{0}={1}", encodedKey, encodedValue);
			else query += String.Format("&{0}={1}", encodedKey, encodedValue);
			return this;
		}

		public HttpRequestBuilder AddToQuery(string key, object value)
		{
			return AddToQuery(key, value.ToString());
		}

		public HttpRequestBuilder SetHeaders(HttpRequestHeaders requestHeaders)
		{
			headers = requestHeaders;
			return this;
		}

		public HttpRequestBuilder SetCustomHeader(string name, string value)
		{
			EnsureHeaders();
			headers.SetCustomHeader(name, value);
			return this;
		}

		#region Known header setters
		public HttpRequestBuilder SetAcceptHeader(string value)
		{
			EnsureHeaders();
			headers.Accept = value;
			return this;
		}

		public HttpRequestBuilder SetAuthorizationHeader(AuthenticationHeaderValue value)
		{
			EnsureHeaders();
			headers.Authorization = value;
			return this;
		}

		public HttpRequestBuilder SetRangeHeader(RangeHeaderValue value)
		{
			EnsureHeaders();
			headers.Range = value;
			return this;
		}

		public HttpRequestBuilder SetReferer(string value)
		{
			EnsureHeaders();
			headers.Referer = value;
			return this;
		}

		public HttpRequestBuilder SetUserAgent(string value)
		{
			EnsureHeaders();
			headers.UserAgent = value;
			return this;
		}
		#endregion

		public HttpRequestBuilder SetBody(IHttpContent requestBody)
		{
			body = requestBody;
			return this;
		}

		private Uri BuildRequestUri()
		{
			if (String.IsNullOrEmpty(query)) 
				return baseUri;
			var builder = new UriBuilder(baseUri)
			{
				Query = query.TrimStart('?')
			};
			return builder.Uri;
		}

		private void EnsureHeaders()
		{
			if (headers == null)
				headers = new HttpRequestHeaders();
		}

		private readonly HttpMethod method;
		private Uri baseUri;
		private string query;
		private HttpRequestHeaders headers;
		private IHttpContent body;
	}
}