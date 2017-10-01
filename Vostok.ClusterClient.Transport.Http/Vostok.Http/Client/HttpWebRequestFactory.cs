using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
    internal static class HttpWebRequestFactory
    {
        private static readonly bool isMono = Type.GetType("Mono.Runtime") != null;

        private static readonly HashSet<string> RestrictedHeaderNames = new HashSet<string>
        {
            HeaderNames.Accept,
            HeaderNames.ContentLength,
            HeaderNames.ContentType,
            HeaderNames.Host,
            HeaderNames.IfModifiedSince,
            HeaderNames.Range,
            HeaderNames.Referer,
            HeaderNames.UserAgent,
        };

        static HttpWebRequestFactory()
        {
            if (isMono)
                return;

            HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            HttpWebRequest.DefaultMaximumResponseHeadersLength = -1;
        }

        public static HttpWebRequest Create(Request request, HttpClientSettings settings, TimeSpan timeout)
        {
            var webRequest = WebRequest.CreateHttp(request.Url.AbsoluteUri);
            TuneRequestParameters(webRequest, settings, timeout);
            SetCredentials(webRequest, settings);
            SetHttpMethod(webRequest, request);
            SetRequestHeaders(webRequest, request);
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
                foreach (var certificate in settings.ClientCertificates)
                    webRequest.ClientCertificates.Add(certificate);
            var timeoutInMilliseconds = Math.Max(1, (int)timeout.TotalMilliseconds);
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

        private static void SetHttpMethod(HttpWebRequest webRequest, Request request)
        {
            webRequest.Method = request.Method;
        }

        private static void SetRequestHeaders(HttpWebRequest webRequest, Request request)
        {
            var headers = request.Headers;
            if (headers == null)
                return;

            if (headers.Accept != null)
                webRequest.Accept = headers.Accept;
            if (headers.Host != null)
                webRequest.Host = headers.Host;

            var ifModifiedSinceHeader = request.GetIfModifiedSinceHeader();
            if (ifModifiedSinceHeader.HasValue)
                webRequest.IfModifiedSince = ifModifiedSinceHeader.Value;

            var range = request.GetRangeHeader();
            if (range != null)
            {
                var rangeValue = range.Value;
                if (rangeValue.From.HasValue && rangeValue.To.HasValue)
                    webRequest.AddRange(rangeValue.Unit, rangeValue.From.Value, rangeValue.To.Value);
                else if (rangeValue.From.HasValue)
                    webRequest.AddRange(rangeValue.From.Value);
                else if (rangeValue.To.HasValue)
                    webRequest.AddRange(-rangeValue.To.Value);
            }

            if (headers.Referer != null)
                webRequest.Referer = headers.Referer;
            if (headers.UserAgent != null)
                webRequest.UserAgent = headers.UserAgent;

            var contentLength = request.GetContentLengthHeader() ?? request.Content?.Length;
            if (contentLength.HasValue)
                webRequest.ContentLength = contentLength.Value;

            if (headers.ContentType != null)
            {
                var contentType = headers.ContentType;
                if (headers.ContentEncoding != null)
                    contentType += $"; charset={headers.ContentEncoding}";

                webRequest.ContentType = contentType;
            }

            foreach (var pair in headers.Where(x => !RestrictedHeaderNames.Contains(x.Name)))
                webRequest.Headers.Set(pair.Name, pair.Value);
        }
    }
}
