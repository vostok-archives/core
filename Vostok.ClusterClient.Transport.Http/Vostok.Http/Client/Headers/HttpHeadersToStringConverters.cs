using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client.Headers
{
    internal static class HttpHeadersToStringConverters
    {
        public static string ToHeaderStringValue(this RangeHeaderValue value)
        {
            var rangeSpecifier = "bytes";
            var ranges = new StringBuilder();
            foreach (var rangeValue in value.Ranges)
            {
                if (rangeValue.From == null && rangeValue.To == null)
                    continue;

                if (ranges.Length > 0)
                {
                    ranges.Append(",");
                }
                else
                {
                    ranges.Append(rangeSpecifier).Append('=');
                }

                if (rangeValue.From.HasValue && rangeValue.To.HasValue)
                {
                    ranges.Append(rangeValue.From.Value).Append('-').Append(rangeValue.To.Value);
                }
                else if (rangeValue.From.HasValue)
                {
                    ranges.Append(rangeValue.From.Value).Append('-');
                }
                else
                {
                    ranges.Append(-rangeValue.To.Value);
                }
            }
            return ranges.ToString();
        }

        public static string ToHeaderStringValue(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return null;
            var dateFormat = new DateTimeFormatInfo();
            return dateTime.ToUniversalTime().ToString("R", dateFormat);
        }
    }
}