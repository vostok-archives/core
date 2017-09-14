using System;
using System.Net;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests
{
	/// <summary>
	/// <para>Представляет клиентский HTTP-запрос, не привязанный к конкретному хосту и директории.</para>
	/// <para>Отличается от <see cref="HttpRequest"/> относительным характером Uri.</para>
	/// <para>Не может иметь абсолютный путь (начинающийся с '/').</para>
	/// <para>Может быть построен удобным образом с помощью <see cref="RelativeHttpRequestBuilder"/>.</para>
	/// </summary>
	public class RelativeHttpRequest : HttpRequestBase
	{
		public RelativeHttpRequest(HttpMethod method, Uri relativeUri, HttpRequestHeaders headers, IHttpContent body)
			: base(method, headers, body)
		{
			Preconditions.EnsureNotNull(relativeUri, "relativeUri");
			Preconditions.EnsureCondition(!relativeUri.IsAbsoluteUri, "relativeUri", "Request URI '{0}' is not relative.", relativeUri);
			Preconditions.EnsureCondition(!relativeUri.OriginalString.StartsWith("/"), "relativeUri", "Request URI '{0}' has absolute path.", relativeUri);
			this.relativeUri = relativeUri;
		}

		public HttpRequest ToHttpRequest(HttpScheme scheme, string host, int port)
		{
			var baseAbsoluteUri = new Uri(String.Format("{0}://{1}:{2}", SchemeUtilities.ToLowercaseString(scheme), host, port));
			return new HttpRequest(method, new Uri(baseAbsoluteUri, relativeUri), headers, body);
		}

		public HttpRequest ToHttpRequest(HttpScheme scheme, IPEndPoint endPoint)
		{
			return ToHttpRequest(scheme, endPoint.Address.ToString(), endPoint.Port);
		}

		public override string ToString()
		{
		    var uriString = relativeUri.ToString();

		    var queryBeginning = uriString.IndexOf('?');

			return ToStringInternal(queryBeginning < 0 ? uriString : uriString.Substring(0, queryBeginning));
		}

	    private readonly Uri relativeUri;
	}
}