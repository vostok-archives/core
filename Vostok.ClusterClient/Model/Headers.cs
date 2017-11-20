using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// <para>Represents a collection of HTTP headers (string key-value pairs).</para>
    /// <para>Every <see cref="Headers"/> object is effectively immutable. Any modifications made via <see cref="Set"/> method produce a new object.</para>
    /// </summary>
    public class Headers : IEnumerable<Header>
    {
        /// <summary>
        /// Represents an empty <see cref="Headers"/> object. Useful to start building headers from scratch.
        /// </summary>
        public static readonly Headers Empty = new Headers(new Header[] {}, 0);

        private readonly Header[] headers;
        private readonly int count;

        public Headers()
            : this(4)
        {
        }

        public Headers(int capacity)
            : this(new Header[capacity], 0)
        {
        }

        internal Headers(Header[] headers, int count)
        {
            this.headers = headers;
            this.count = count;
        }

        /// <summary>
        /// Returns the count of headers in this <see cref="Headers"/> object.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Returns the names of all headers contained in this <see cref="Headers"/> object.
        /// </summary>
        [NotNull]
        public IEnumerable<string> Names => this.Select(header => header.Name);

        /// <summary>
        /// Returns the values of all headers contained in this <see cref="Headers"/> object.
        /// </summary>
        [NotNull]
        public IEnumerable<string> Values => this.Select(header => header.Value);

        /// <summary>
        /// Attempts to fetch the value of header with given name.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <returns>Header value if found, <c>null</c> otherwise.</returns>
        [CanBeNull]
        public string this[string name] => Find(name, out _)?.Value;

        /// <summary>
        /// <para>Produces a new <see cref="Headers"/> instance where the header with given name will have given value.</para>
        /// <para>If the header does not exist in current instance, it will be added to new one.</para>
        /// <para>If the header exists in current instance, it will be overwritten in new one.</para>
        /// <para>Current instance is not modified in any case.</para>
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value. ToString() is used to obtain string value.</param>
        /// <returns>A new <see cref="Headers"/> object with updated header value.</returns>
        [Pure]
        [NotNull]
        public Headers Set<T>([NotNull] string name, [NotNull] T value)
        {
            return Set(name, value.ToString());
        }

        /// <summary>
        /// <para>Produces a new <see cref="Headers"/> instance where the header with given name will have given value.</para>
        /// <para>If the header does not exist in current instance, it will be added to new one.</para>
        /// <para>If the header exists in current instance, it will be overwritten in new one.</para>
        /// <para>Current instance is not modified in any case.</para>
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        /// <returns>A new <see cref="Headers"/> object with updated header value.</returns>
        [Pure]
        [NotNull]
        public Headers Set([NotNull] string name, [NotNull] string value)
        {
            var oldHeader = Find(name, out var oldHeaderIndex);
            var newHeader = new Header(name, value);

            Header[] newHeaders;

            if (oldHeader != null)
            {
                if (oldHeader.Equals(newHeader))
                    return this;

                newHeaders = ReallocateArray(headers.Length);
                newHeaders[oldHeaderIndex] = newHeader;
                return new Headers(newHeaders, count);
            }

            if (headers.Length == count)
            {
                newHeaders = ReallocateArray(Math.Max(4, headers.Length*2));
                newHeaders[count] = newHeader;
                return new Headers(newHeaders, count + 1);
            }

            if (Interlocked.CompareExchange(ref headers[count], newHeader, null) != null)
            {
                newHeaders = ReallocateArray(headers.Length);
                newHeaders[count] = newHeader;
                return new Headers(newHeaders, count + 1);
            }

            return new Headers(headers, count + 1);
        }

        /// <summary>
        /// <para>Produces a new <see cref="Headers"/> instance where the header with given name will be removed.</para>
        /// <para>If the header does not exist in current instance, the same <see cref="Headers"/> object will be returned instead.</para>
        /// <para>Current instance is not modified in any case.</para>
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <returns>A new <see cref="Headers"/> object without a header with given name.</returns>
        [Pure]
        [NotNull]
        public Headers Remove([NotNull] string name)
        {
            var oldHeader = Find(name, out var oldHeaderIndex);
            if (oldHeader == null)
                return this;

            var newHeaders = new Header[headers.Length - 1];

            if (oldHeaderIndex > 0)
                Array.Copy(headers, 0, newHeaders, 0, oldHeaderIndex);

            if (oldHeaderIndex < count - 1)
                Array.Copy(headers, oldHeaderIndex + 1, newHeaders, oldHeaderIndex, count - oldHeaderIndex - 1);

            return new Headers(newHeaders, count - 1);
        }

        public override string ToString()
        {
            if (count == 0)
                return string.Empty;

            var builder = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                var header = headers[i];

                builder.Append(header.Name);
                builder.Append(": ");
                builder.Append(header.Value);

                if (i < count - 1)
                    builder.AppendLine();
            }

            return builder.ToString();
        }

        public IEnumerator<Header> GetEnumerator()
        {
            for (var i = 0; i < count; i++)
                yield return headers[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [CanBeNull]
        private Header Find(string name, out int index)
        {
            for (var i = 0; i < count; i++)
            {
                var header = headers[i];
                if (header.Name.Equals(name, StringComparison.Ordinal))
                {
                    index = i;
                    return header;
                }
            }

            index = -1;
            return null;
        }

        [NotNull]
        private Header[] ReallocateArray(int capacity)
        {
            var reallocated = new Header[capacity];

            Array.Copy(headers, 0, reallocated, 0, count);

            return reallocated;
        }

        #region Specific header getters

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Accept"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Accept => this[HeaderNames.Accept];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Age"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Age => this[HeaderNames.Age];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Authorization"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Authorization => this[HeaderNames.Authorization];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.ContentEncoding"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string ContentEncoding => this[HeaderNames.ContentEncoding];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.ContentLength"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string ContentLength => this[HeaderNames.ContentLength];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.ContentType"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string ContentType => this[HeaderNames.ContentType];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.ContentRange"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string ContentRange => this[HeaderNames.ContentRange];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Date"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Date => this[HeaderNames.Date];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.ETag"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string ETag => this[HeaderNames.ETag];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Host"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Host => this[HeaderNames.Host];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.LastModified"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string LastModified => this[HeaderNames.LastModified];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Location"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Location => this[HeaderNames.Location];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Range"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Range => this[HeaderNames.Range];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Referer"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Referer => this[HeaderNames.Referer];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.RetryAfter"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string RetryAfter => this[HeaderNames.RetryAfter];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.Server"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string Server => this[HeaderNames.Server];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.TransferEncoding"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string TransferEncoding => this[HeaderNames.TransferEncoding];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.UserAgent"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string UserAgent => this[HeaderNames.UserAgent];

        /// <summary>
        /// Returns the value of <see cref="HeaderNames.WWWAuthenticate"/> header or <c>null</c> if it's not specified.
        /// </summary>
        [CanBeNull]
        public string WWWAuthenticate => this[HeaderNames.WWWAuthenticate];

        #endregion
    }
}
