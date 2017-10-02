using System;
using System.Net;
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
            SetHeaders(webRequest, request);

            return webRequest;
        }

        private static void TuneRequestParameters(HttpWebRequest webRequest, TimeSpan timeout)
        {
            webRequest.Expect = null;
            webRequest.KeepAlive = true;
            webRequest.Pipelined = true;
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
                webRequest.Headers.Set(header.Name, header.Value);
            }
        }
    }
}