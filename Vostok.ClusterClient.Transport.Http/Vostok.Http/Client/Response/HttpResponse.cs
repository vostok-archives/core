using System;
using System.Net;
using System.Text;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Response
{
	/// <summary>
	/// Представляет ответ сервера на клиентский HTTP-запрос.
	/// </summary>
	public class HttpResponse
	{
		public HttpResponse(HttpResponseCode code, HttpResponseHeaders headers, ByteArrayContent body, Version protocolVersion)
		{
			this.code = code;
			this.headers = headers;
			this.body = body;
			this.protocolVersion = protocolVersion;
		}

		public HttpResponse(HttpResponseCode code, HttpResponseHeaders headers, ByteArrayContent body)
			: this(code, headers, body, null) { }

		public HttpResponse(HttpResponseCode code, HttpResponseHeaders headers)
			: this(code, headers, null, null) { }

		public HttpResponse(HttpResponseCode code, ByteArrayContent body)
			: this(code, null, body, null) { }

		public HttpResponse(HttpResponseCode code)
			: this(code, null, null, null) { }

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.AppendFormat("HTTP/{0} {1} {2}", ProtocolVersion, (int) code, code);
			if (headers != null)
			{
				builder.AppendLine();
				builder.Append(headers);
			}
			return builder.ToString();
		}

		public HttpResponseCode Code
		{
			get { return code; }
		}

		public HttpResponseHeaders Headers
		{
			get { return headers ?? HttpResponseHeaders.Empty; }
		}

		/// <summary>
		/// <para>Тело ответа, возвращенного сервером.</para>
		/// <para>Если сервер не возвращал контент, это свойство вернет пустой ByteArrayContent с длиной 0.</para>
		/// <para>Длина внутреннего буфера в ByteArrayContent может быть больше длины актуальных данных, поэтому используйте свойство <see cref="IHttpContent.Length"/>, чтобы узнать их реальное количество.</para>
		/// </summary>
		public ByteArrayContent Body
		{
			get { return body ?? ByteArrayContent.Empty; }
		}

		public Version ProtocolVersion
		{
			get { return protocolVersion ?? HttpVersion.Version11; }
		}

		/// <summary>
		/// Возвращает true, если код ответа находится в семействе 2xx (200 - 206).
		/// </summary>
		public bool IsSuccessful
		{
			get { return Code.IsSuccessful();}
		}

		private readonly HttpResponseCode code;
		private readonly HttpResponseHeaders headers;
		private readonly ByteArrayContent body;
		private readonly Version protocolVersion;
	}
}