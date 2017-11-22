using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class HttpWebRequestFactory
    {
        static HttpWebRequestFactory()
        {
            HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            HttpWebRequest.DefaultMaximumResponseHeadersLength = -1;

            ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
            ServicePointManager.CheckCertificateRevocationList = false;
        }

        public static HttpWebRequest Create(Request request, TimeSpan timeout)
        {
            var webRequest = WebRequest.CreateHttp(request.Url);

            TuneRequestParameters(webRequest, timeout);
            SetHttpMethod(webRequest, request);
            SetContentLength(webRequest, request);
            SetHeaders(webRequest, request);

            return webRequest;
        }

        private static void TuneRequestParameters(HttpWebRequest webRequest, TimeSpan timeout)
        {
            webRequest.Expect = null;
            webRequest.KeepAlive = true;
            webRequest.Pipelined = true;
            // ReSharper disable once AssignNullToNotNullAttribute
            webRequest.Proxy = null;
            webRequest.AllowAutoRedirect = false;
            webRequest.AllowWriteStreamBuffering = false;
            webRequest.AllowReadStreamBuffering = false;
            webRequest.AuthenticationLevel = AuthenticationLevel.None;
            webRequest.AutomaticDecompression = DecompressionMethods.None;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.ServicePoint.ConnectionLimit = 10000;
            webRequest.ServicePoint.UseNagleAlgorithm = false;
            webRequest.ServicePoint.ReceiveBufferSize = 16 * 1024;

            var timeoutInMilliseconds = Math.Max(1, (int)timeout.TotalMilliseconds);
            webRequest.Timeout = timeoutInMilliseconds;
            webRequest.ReadWriteTimeout = timeoutInMilliseconds;
        }

        private static void SetHttpMethod(HttpWebRequest webRequest, Request request)
        {
            webRequest.Method = request.Method;
        }

        private static void SetHeaders(HttpWebRequest webRequest, Request request)
        {
            if (request.Headers == null)
                return;

            foreach (var header in request.Headers)
            {
                if (TryHandleSpecialHeader(webRequest, header))
                    continue;

                webRequest.Headers.Set(header.Name, header.Value);
            }
        }

        private static void SetContentLength(HttpWebRequest webRequest, Request request)
        {
            webRequest.ContentLength = request.Content?.Length ?? 0;
        }

        private static bool TryHandleSpecialHeader(HttpWebRequest webRequest, Header header)
        {
            if (header.Name.Equals(HeaderNames.Accept))
            {
                webRequest.Accept = header.Value;
                return true;
            }

            if (header.Name.Equals(HeaderNames.ContentLength))
                return true;

            if (header.Name.Equals(HeaderNames.ContentType))
            {
                webRequest.ContentType = header.Value;
                return true;
            }

            if (header.Name.Equals(HeaderNames.Host))
            {
                webRequest.Host = header.Value;
                return true;
            }

            if (header.Name.Equals(HeaderNames.IfModifiedSince))
            {
                webRequest.IfModifiedSince = DateTime.Parse(header.Value);
                return true;
            }

            if (header.Name.Equals(HeaderNames.Range))
            {
                var ranges = RangeHeaderValue.Parse(header.Value);

                foreach (var range in ranges.Ranges)
                {
                    webRequest.AddRange(ranges.Unit, range.From ?? 0, range.To ?? 0);
                }

                return true;
            }

            if (header.Name.Equals(HeaderNames.Referer))
            {
                webRequest.Referer = header.Value;
                return true;
            }

            if (header.Name.Equals(HeaderNames.UserAgent))
            {
                webRequest.UserAgent = header.Value;
                return true;
            }

            return false;
        }
    }
}