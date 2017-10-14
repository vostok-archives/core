using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    public static class RequestHeadersExtensions
    {
        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.Accept"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithAcceptHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.Accept, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.AcceptCharset"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithAcceptCharsetHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.AcceptCharset, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.AcceptCharset"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithAcceptCharsetHeader([NotNull] this Request request, [NotNull] Encoding value)
        {
            return WithAcceptCharsetHeader(request, value.WebName);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.AcceptEncoding"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithAcceptEncodingHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.AcceptEncoding, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.Authorization"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithAuthorizationHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.Authorization, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.Authorization"/> header composed from given <paramref name="scheme"/> and <paramref name="parameter"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithAuthorizationHeader([NotNull] this Request request, [NotNull] string scheme, [NotNull] string parameter)
        {
            return WithAuthorizationHeader(request, scheme + " " + parameter);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.Authorization"/> basic auth header composed from given <paramref name="user"/> and <paramref name="password"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithBasicAuthorizationHeader([NotNull] this Request request, [NotNull] string user, [NotNull] string password)
        {
            return WithAuthorizationHeader(request, "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + password)));
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.ContentEncoding"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContentEncodingHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.ContentEncoding, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.ContentType"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContentTypeHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.ContentType, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.ContentRange"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContentRangeHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.ContentRange, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.ContentRange"/> header set according to given <paramref name="length"/> value.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContentRangeHeader([NotNull] this Request request, long length, [NotNull] string unit = "bytes")
        {
            return WithContentRangeHeader(request, $"{unit} */{length}");
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.ContentRange"/> header set according to given <paramref name="from"/> and <paramref name="to"/> values.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContentRangeHeader([NotNull] this Request request, long from, long to, [NotNull] string unit = "bytes")
        {
            return WithContentRangeHeader(request, $"{unit} {from}-{to}/*");
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.ContentRange"/> header set according to given <paramref name="from"/>, <paramref name="to"/> and <paramref name="length"/> values.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContentRangeHeader([NotNull] this Request request, long from, long to, long length, [NotNull] string unit = "bytes")
        {
            return WithContentRangeHeader(request, $"{unit} {from}-{to}/{length}");
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.IfMatch"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithIfMatchHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.IfMatch, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.IfNoneMatch"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithIfNoneMatchHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.IfNoneMatch, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.IfModifiedSince"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithIfModifiedSinceHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.IfModifiedSince, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.IfModifiedSince"/> header set to given <paramref name="value"/> formatted with <paramref name="format"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithIfModifiedSinceHeader([NotNull] this Request request, DateTime value, string format = "R")
        {
            return WithIfModifiedSinceHeader(request, value.ToString(format));
        }

        /// <summary>
        /// <para>Returns a <see cref="HeaderNames.IfModifiedSince"/> header formatted with <paramref name="format"/>.</para>
        /// </summary>
        [Pure]
        public static DateTime? GetIfModifiedSinceHeader([NotNull] this Request request, string format = "R")
        {
            var strHeader = request.Headers?[HeaderNames.IfModifiedSince];
            return strHeader == null
                ? (DateTime?)null
                : DateTime.ParseExact(strHeader, format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.Range"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithRangeHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.Range, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.Range"/> header composed from given <paramref name="from"/>, <paramref name="to"/> and <paramref name="unit"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithRangeHeader([NotNull] this Request request, long? from, long? to, [NotNull] string unit = "bytes")
        {
            if (from == null && to == null)
                throw new ArgumentException($"At least one of '{nameof(from)}' and '{nameof(to)}' parameters must be non-null.");

            return WithRangeHeader(request, $"{unit}={from}-{to}");
        }

        /// <summary>
        /// <para>Returns a <see cref="HeaderNames.Range"/> header/>.</para>
        /// </summary>
        [Pure]
        public static (long? From, long? To, string Unit)? GetRangeHeader([NotNull] this Request request)
        {
            var strHeader = request.Headers?[HeaderNames.Range];

            if (strHeader == null)
                return null;

            var unitsWithRange = strHeader.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);

            if(unitsWithRange.Length != 2)
                throw new ArgumentException($"Invalid {nameof(HeaderNames.Range)} header - {strHeader}");

            var unit = unitsWithRange[0];
            var ranges = unitsWithRange[1].Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries);

            if (ranges.Length != 2)
                throw new ArgumentException($"Invalid {nameof(HeaderNames.Range)} header - {strHeader}");

            var from = string.IsNullOrEmpty(ranges[0]) ? (long?)null : long.Parse(ranges[0]);
            var to = string.IsNullOrEmpty(ranges[1]) ? (long?)null : long.Parse(ranges[1]);

            return (from, to, unit);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with <see cref="HeaderNames.UserAgent"/> header set to given <paramref name="value"/>.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithUserAgentHeader([NotNull] this Request request, [NotNull] string value)
        {
            return request.WithHeader(HeaderNames.UserAgent, value);
        }

        /// <summary>
        /// <para>Returns a new <see cref="Request"/> instance with the header specified in <paramref name="name"/>, updated with specified <paramref name="value"/> and <paramref name="quality"/>.</para>
        /// <para>If header doesn't exist, it will be created.</para>
        /// <para>See <see cref="Request.WithHeader"/> for more details.</para>
        /// </summary>
        [Pure]
        [NotNull]
        public static Request AppendToHeaderWithQuality([NotNull] this Request request, [NotNull] string name, [NotNull] string value, decimal quality = 1)
        {
            var currentValue = request.Headers?[name];
            if (string.IsNullOrEmpty(currentValue))
                return request.WithHeader(name, new HeaderValueWithQuality(value, quality));

            var valueCollection = HeaderValuesWithQualityCollection.Parse(currentValue);
            valueCollection.Add(value, quality);
            return request.WithHeader(name, valueCollection.ToString());
        }

        /// <summary>
        /// <para>Returns a <see cref="HeaderNames.ContentLength"/> header/>.</para>
        /// </summary>
        [Pure]
        public static long? GetContentLengthHeader([NotNull] this Request request)
        {
            var strHeader = request.Headers?[HeaderNames.ContentLength];
            return strHeader == null
                ? (long?)null
                : long.Parse(strHeader);
        }
    }
}
