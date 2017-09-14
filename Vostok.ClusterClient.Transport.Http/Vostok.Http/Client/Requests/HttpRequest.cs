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

		public override string ToString()
		{
			return ToStringInternal(absoluteUri.GetLeftPart(UriPartial.Path));
		}

		public Uri AbsoluteUri => absoluteUri;

	    private readonly Uri absoluteUri;
	}
}