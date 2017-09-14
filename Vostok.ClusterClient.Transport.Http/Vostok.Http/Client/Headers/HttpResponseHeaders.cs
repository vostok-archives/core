using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Headers;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers
{
	/// <summary>
	/// Клиентское представление заголовков, возвращаемых сервером в ответ на запрос.
	/// </summary>
	public class HttpResponseHeaders : ReadonlyHeadersCollection
	{
		public HttpResponseHeaders(NameValueCollection headers) 
			: base(headers) { }

		/// <summary>
		/// Возвращает указанное сервером значение ContentLength, или -1 в случае его отсутствия.
		/// </summary>
		public long ContentLength
		{
			get
			{
				string value = headers[HttpHeaderNames.ContentLength];
				if (String.IsNullOrEmpty(value))
					return -1;
				return Int64.Parse(value);
			}
		}

		public TimeSpan? Age
		{
			get
			{
				string value = headers[HttpHeaderNames.Age];
				if (String.IsNullOrEmpty(value))
					return null;
				return TimeSpan.FromSeconds(Int64.Parse(value));
			}
		}

		public TimeSpan? RetryAfter
		{
			get
			{
				string value = headers[HttpHeaderNames.RetryAfter];
				if (String.IsNullOrEmpty(value))
					return null;
				return TimeSpan.FromSeconds(Int64.Parse(value));
			}
		}

		public DateTime? Date
		{
			get
			{
				string value = headers[HttpHeaderNames.Date];
				if (String.IsNullOrEmpty(value))
					return null;
				return DateTime.Parse(value);
			}
		}

		public EntityTagHeaderValue ETag
		{
			get
			{
				string value = headers[HttpHeaderNames.ETag];
				return String.IsNullOrEmpty(value) 
					? null 
					: EntityTagHeaderValue.Parse(value);
			}
		}

		public Uri Location
		{
			get
			{
				string value = headers[HttpHeaderNames.Location];
				return String.IsNullOrEmpty(value)
					? null
					: new Uri(value);
			}
		}

		public string Server
		{
			get { return headers[HttpHeaderNames.Server]; }
		}

		public AuthenticationHeaderValue WWWAuthenticate
		{
			get
			{
				string value = headers[HttpHeaderNames.WWWAuthenticate];
				return String.IsNullOrEmpty(value) 
					? null 
					: AuthenticationHeaderValue.Parse(value);
			}
		}

		public static HttpResponseHeaders Empty = new HttpResponseHeaders(new WebHeaderCollection());
	}
}