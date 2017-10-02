using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class SystemNetHttpHeaderUtilities
    {
        private static readonly HashSet<string> contentHeaders =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
            {
                HeaderNames.ContentLength,
                HeaderNames.ContentType,
                HeaderNames.ContentEncoding,
                HeaderNames.ContentLanguage,
                HeaderNames.ContentLocation,
                HeaderNames.ContentMD5,
                HeaderNames.ContentRange,
                HeaderNames.Allow,
                HeaderNames.ContentDisposition,
                HeaderNames.Expires,
                HeaderNames.LastModified
            };

        public static bool IsContentHeader(string header)
        {
            return contentHeaders.Contains(header);
        }

        public static Headers Append(this Headers to, HttpHeaders from)
        {
            foreach (var header in GetRawHeaders(from))
            {
                to = to.Set(header.Key, header.Value);
            }

            return to;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetRawHeaders(HttpHeaders headers)
        {
            if (headers == null)
                yield break;

            //@ezsilmar There is no way to get raw header values from HttpClient through public interface.
            foreach (var pair in headers)
            {
                var separator = GetSeparatorByHeaderName(pair.Key);
                var headerValue = string.Join(separator, pair.Value);
                yield return new KeyValuePair<string, string>(pair.Key, headerValue);
            }
        }

        private static string GetSeparatorByHeaderName(string header)
        {
            //@ezsilmar We can't choose the correct separator through public interface of client
            //However, in HttpClient code the only header which uses space instead of comma is User-Agent
            return header.Equals(HeaderNames.UserAgent, StringComparison.InvariantCultureIgnoreCase) ? " " : ", ";
        }
    }
}