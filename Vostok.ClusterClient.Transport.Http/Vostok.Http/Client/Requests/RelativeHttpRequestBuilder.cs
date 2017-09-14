using System;
using System.Net.Http.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;
using HttpRequestHeaders = Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers.HttpRequestHeaders;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests
{
	public class RelativeHttpRequestBuilder
	{
		private RelativeHttpRequestBuilder(HttpMethod method, string pathAndQuery)
		{
			this.method = method;
			this.pathAndQuery = pathAndQuery;
			if (pathAndQuery.Contains("?"))
				hasQuery = true;
		}

		public static RelativeHttpRequestBuilder StartNew(HttpMethod method, string pathAndQuery = null)
		{
			return new RelativeHttpRequestBuilder(method, pathAndQuery ?? String.Empty);
		}

		public RelativeHttpRequest Build()
		{
			return new RelativeHttpRequest(method, new Uri(pathAndQuery, UriKind.Relative), headers, body);
		}

	    public RelativeHttpRequestBuilder AddPathSegment(string segment, bool encode = true)
	    {
            if (segment.Contains("/"))
                throw new ArgumentException("A path segment cannot contain '/' character.");

            if (hasQuery)
                throw new InvalidOperationException("Can't add any path segments when a relative request already has query string.");

            if (encode)
                segment = UrlEncodingUtility.UrlEncode(segment);

	        pathAndQuery = String.IsNullOrEmpty(pathAndQuery) 
                ? segment 
                : pathAndQuery.TrimEnd('/') + '/' + segment;

	        return this;
	    }

		public RelativeHttpRequestBuilder AddToQuery(string key, string value)
		{
			string encodedKey = UrlEncodingUtility.UrlEncode(key, EncodingFactory.GetDefault());
			string encodedValue = UrlEncodingUtility.UrlEncode(value, EncodingFactory.GetDefault());
			if (hasQuery)
				pathAndQuery += String.Format("&{0}={1}", encodedKey, encodedValue);
			else
			{
				pathAndQuery += String.Format("?{0}={1}", encodedKey, encodedValue);
				hasQuery = true;
			}
			return this;
		}

		public RelativeHttpRequestBuilder AddToQuery(string key, object value)
		{
			return AddToQuery(key, value.ToString());
		}

		public RelativeHttpRequestBuilder SetHeaders(HttpRequestHeaders requestHeaders)
		{
			headers = requestHeaders;
			return this;
		}

		public RelativeHttpRequestBuilder SetCustomHeader(string name, string value)
		{
			EnsureHeaders();
			headers.SetCustomHeader(name, value);
			return this;
		}

		#region Known header setters
		public RelativeHttpRequestBuilder SetAcceptHeader(string value)
		{
			EnsureHeaders();
			headers.Accept = value;
			return this;
		}

		public RelativeHttpRequestBuilder SetAuthorizationHeader(AuthenticationHeaderValue value)
		{
			EnsureHeaders();
			headers.Authorization = value;
			return this;
		}

		public RelativeHttpRequestBuilder SetRangeHeader(RangeHeaderValue value)
		{
			EnsureHeaders();
			headers.Range = value;
			return this;
		}

		public RelativeHttpRequestBuilder SetReferer(string value)
		{
			EnsureHeaders();
			headers.Referer = value;
			return this;
		}

		public RelativeHttpRequestBuilder SetUserAgent(string value)
		{
			EnsureHeaders();
			headers.UserAgent = value;
			return this;
		}
		#endregion

		public RelativeHttpRequestBuilder SetBody(IHttpContent requestBody)
		{
			body = requestBody;
			return this;
		}

		private void EnsureHeaders()
		{
			if (headers == null)
				headers = new HttpRequestHeaders();
		}

		private readonly HttpMethod method;
		private string pathAndQuery;
		private HttpRequestHeaders headers;
		private IHttpContent body;
		private bool hasQuery;
	}
}