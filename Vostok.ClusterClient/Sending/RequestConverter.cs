using System;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Sending
{
    internal class RequestConverter : IRequestConverter
    {
        private readonly ILog log;

        public RequestConverter(ILog log)
        {
            this.log = log;
        }

        public Request TryConvertToAbsolute(Request relativeRequest, Uri replica)
        {
            if (!replica.IsAbsoluteUri)
            {
                LogReplicaUrlNotAbsolute(replica);
                return null;
            }

            if (!string.IsNullOrEmpty(replica.Query))
            {
                LogReplicaUrlContainsQuery(replica);
                return null;
            }

            var requestUrl = relativeRequest.Url;
            if (requestUrl.IsAbsoluteUri)
            {
                LogRequestHasAbsoluteUrl(requestUrl);
                return null;
            }

            try
            {
                var convertedUrl = ConvertUrl(requestUrl, replica);
                var convertedRequest = relativeRequest.WithUrl(convertedUrl);

                return convertedRequest;
            }
            catch (Exception error)
            {
                LogUrlConversionException(replica, requestUrl, error);
                return null;
            }
        }

        private static Uri ConvertUrl(Uri requestUrl, Uri replica)
        {
            var baseUrl = replica.OriginalString;
            var baseUrlEndsWithSlash = baseUrl.EndsWith("/");

            var appendedUrl = requestUrl.OriginalString;
            var appendedUrlStartsWithSlash = appendedUrl.StartsWith("/");

            if (baseUrlEndsWithSlash && appendedUrlStartsWithSlash)
            {
                appendedUrl = appendedUrl.Substring(1);
            }

            if (!baseUrlEndsWithSlash && !appendedUrlStartsWithSlash)
            {
                appendedUrl = "/" + appendedUrl;
            }

            return new Uri(baseUrl + appendedUrl, UriKind.Absolute);
        }

        #region Logging 

        private void LogReplicaUrlNotAbsolute(Uri replica)
        {
            log.Error($"Given replica url is not absolute: '{replica}'. Absolute url is expected here.");
        }

        private void LogReplicaUrlContainsQuery(Uri replica)
        {
            log.Error($"Replica url contains query parameters: '{replica}'. No query parameters are allowed for replicas.");
        }

        private void LogRequestHasAbsoluteUrl(Uri requestUrl)
        {
            log.Error($"Request contains absolute url: '{requestUrl}'. Relative url is expected instead.");
        }

        private void LogUrlConversionException(Uri replica, Uri requestUrl, Exception error)
        {
            log.Error($"Failed to merge replica url '{replica}' and request url '{requestUrl}'.", error);
        }

        #endregion
    }
}
