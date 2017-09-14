using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Requests;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Headers;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.HttpContent;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Common.Utility;
using Vostok.Tracing;
using HttpRequestHeaders = Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers.HttpRequestHeaders;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
	internal static class HttpWebRequestFactory
	{
		private static readonly bool isMono = Type.GetType("Mono.Runtime") != null;

	    static HttpWebRequestFactory()
	    {
			if (!isMono)
			{
				HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
				HttpWebRequest.DefaultMaximumResponseHeadersLength = -1;
			}
	    }

		public static HttpWebRequest Create(HttpRequest request, HttpClientSettings settings, TimeSpan timeout)
		{
			var webRequest = WebRequest.CreateHttp(request.AbsoluteUri);
			TuneRequestParameters(webRequest, settings, timeout);
		    SetCredentials(webRequest, settings);
		    SetHttpMethod(webRequest, request);
		    SetRequestHeaders(webRequest, request, timeout);
		    SetTracingHeaders(webRequest);
		    SetContentHeaders(webRequest, request);
			return webRequest;
		}

	    private static void TuneRequestParameters(HttpWebRequest webRequest, HttpClientSettings settings, TimeSpan timeout)
		{
			webRequest.Expect = null;
			webRequest.KeepAlive = settings.KeepAlive;
			webRequest.Pipelined = true;
			webRequest.Proxy = settings.Proxy;
			webRequest.AllowAutoRedirect = settings.AllowAutoRedirect;
		    webRequest.AllowWriteStreamBuffering = settings.SendDomainIdentity;
			webRequest.AllowReadStreamBuffering = false;
			webRequest.AuthenticationLevel = AuthenticationLevel.None;
			webRequest.AutomaticDecompression = DecompressionMethods.None;
			webRequest.ServicePoint.Expect100Continue = false;
			webRequest.ServicePoint.ConnectionLimit = settings.MaxConnectionsPerEndpoint;
			webRequest.ServicePoint.UseNagleAlgorithm = settings.UseNagleAlgorithm;
			if (!isMono)
				webRequest.ServicePoint.ReceiveBufferSize = 16 * 1024;
			if (settings.ClientCertificates != null)
				foreach (X509Certificate2 certificate in settings.ClientCertificates)
					webRequest.ClientCertificates.Add(certificate);
			int timeoutInMilliseconds = Math.Max(1, (int) timeout.TotalMilliseconds);
			webRequest.Timeout = timeoutInMilliseconds;
			webRequest.ReadWriteTimeout = timeoutInMilliseconds;
		}

	    private static void SetCredentials(HttpWebRequest webRequest, HttpClientSettings settings)
	    {
	        if (settings.SendDomainIdentity)
	        {
	            if (settings.DomainIdentity != null)
	            {
	                webRequest.Credentials = settings.DomainIdentity;
	            }
	            else
	            {
	                webRequest.UseDefaultCredentials = true;
	            }
	        }
	    }

	    private static void SetHttpMethod(HttpWebRequest webRequest, HttpRequest request)
		{
			webRequest.Method = MethodUtilities.GetString(request.Method);
		}

		private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequest request, TimeSpan timeout)
		{
			// (iloktionov): Укажем таймаут, с которым выполняем запрос, и свою identity.
			webRequest.Headers.Set(HttpHeaderNames.XKonturRequestTimeout, timeout.Ticks.ToString());
			webRequest.Headers.Set(HttpHeaderNames.XKonturClientIdentity, UrlEncodingUtility.UrlEncode(HttpClientIdentity.Get(), EncodingFactory.GetDefault()));

			if (!request.HasHeaders())
				return;
			HttpRequestHeaders headers = request.Headers;

			// (iloktionov): Проставляем особые заголовки (часть из них ставятся через свойства HttpWebRequest).
			if (headers.Accept != null)
				webRequest.Accept = headers.Accept;
			if (headers.AcceptCharset != null)
				webRequest.Headers.Set(HttpHeaderNames.AcceptCharset, headers.AcceptCharset.WebName);
			if (headers.Authorization != null)
				webRequest.Headers.Set(HttpHeaderNames.Authorization, headers.Authorization.ToString());
			if (headers.Host != null)
				webRequest.Host = headers.Host;
			if (headers.IfMatch != null)
				webRequest.Headers.Set(HttpHeaderNames.IfMatch, headers.IfMatch.ToString());
			if (headers.IfModifiedSince.HasValue)
				webRequest.IfModifiedSince = headers.IfModifiedSince.Value;
			if (headers.Range != null)
				foreach (RangeItemHeaderValue rangeValue in headers.Range.Ranges)
				{
					if (rangeValue.From.HasValue && rangeValue.To.HasValue)
						webRequest.AddRange(rangeValue.From.Value, rangeValue.To.Value);
					else if (rangeValue.From.HasValue)
						webRequest.AddRange(rangeValue.From.Value);
					else if (rangeValue.To.HasValue)
						webRequest.AddRange(-rangeValue.To.Value);
				}
			if (headers.Referer != null)
				webRequest.Referer = headers.Referer;
			if (headers.UserAgent != null)
				webRequest.UserAgent = headers.UserAgent;

			// (iloktionov): Далее - остальные заголовки.
			if (headers.CustomHeaders != null)
				foreach (var pair in headers.CustomHeaders)
					webRequest.Headers.Set(pair.Key, pair.Value);
		}

		private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequest request)
		{
			if (request.Body == null)
				webRequest.ContentLength = 0;
			else
			{
				IHttpContent body = request.Body;
				webRequest.ContentLength = body.Length;
				string contentType = (body.ContentType ?? ContentType.OctetStream).ToString();
				if (body.Charset != null)
					contentType += "; charset=" + body.Charset.WebName;
				webRequest.ContentType = contentType;
				if (body.ContentRange != null)
					webRequest.Headers.Set(HttpHeaderNames.ContentRange, body.ContentRange.ToString());
			}
		}

	    private static void SetTracingHeaders(HttpWebRequest webRequest)
	    {
	        var traceContext = TraceContext.Current;
	        if (traceContext == null)
	            return;

            webRequest.Headers.Set(TraceHttpHeaders.XKonturTraceId, traceContext.TraceId);
            webRequest.Headers.Set(TraceHttpHeaders.XKonturTraceSpanId, traceContext.SpanId);

            if (!String.IsNullOrEmpty(traceContext.ParentSpanId))
                webRequest.Headers.Set(TraceHttpHeaders.XKonturTraceParentSpanId, traceContext.ParentSpanId);

            if (!String.IsNullOrEmpty(traceContext.ProfileId))
                webRequest.Headers.Set(TraceHttpHeaders.XKonturTraceProfileId, traceContext.ProfileId);

            if (traceContext.IsSampled)
                webRequest.Headers.Set(TraceHttpHeaders.XKonturTraceIsSampled, "true");
	    }
	}
}
