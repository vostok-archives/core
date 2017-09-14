using System;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests
{
	/// <summary>
	/// <para>Представляет клиентский HTTP-запрос. Обязательно содержит хост, на который направлен запрос.</para>
	/// <para>Может быть построен удобным образом с помощью <see cref="HttpRequestBuilder"/>.</para>
	/// </summary>
	public class HttpRequest : HttpRequestBase
	{
		public HttpRequest(HttpMethod method, Uri absoluteUri, HttpRequestHeaders headers, IHttpContent body)
			: base (method, headers, body)
		{
			Preconditions.EnsureNotNull(absoluteUri, "absoluteUri");
			Preconditions.EnsureCondition(absoluteUri.IsAbsoluteUri, "absoluteUri", "Request URI '{0}' is not absolute.", absoluteUri);
			Preconditions.EnsureCondition(SchemeUtilities.UriHasCorrectScheme(absoluteUri), "absoluteUri", "Request URI has incorrect scheme: '{0}'", absoluteUri.Scheme);
			this.absoluteUri = absoluteUri;
		}

		public HttpRequest(HttpMethod method, string absoluteUri, HttpRequestHeaders headers, IHttpContent body)
			: this (method, new Uri(absoluteUri, UriKind.Absolute), headers, body) { }

		public HttpRequest(HttpMethod method, Uri absoluteUri, HttpRequestHeaders headers)
			: this (method, absoluteUri, headers, null) { }

		public HttpRequest(HttpMethod method, string absoluteUri, HttpRequestHeaders headers)
			: this(method, new Uri(absoluteUri, UriKind.Absolute), headers) { }

		public HttpRequest(HttpMethod method, Uri absoluteUri, IHttpContent body)
			: this(method, absoluteUri, null, body) { }

		public HttpRequest(HttpMethod method, string absoluteUri, IHttpContent body)
			: this(method, new Uri(absoluteUri, UriKind.Absolute), body) { }

		public HttpRequest(HttpMethod method, Uri absoluteUri)
			: this(method, absoluteUri, null, null) { }

		public HttpRequest(HttpMethod method, string absoluteUri)
			: this(method, new Uri(absoluteUri, UriKind.Absolute)) { }

		public override string ToString()
		{
			return ToStringInternal(absoluteUri.GetLeftPart(UriPartial.Path));
		}

		public Uri AbsoluteUri
		{
			get { return absoluteUri; }
		}

		private readonly Uri absoluteUri;
	}
}