using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Vostok.Commons.Extensions.Uri;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// <para>Represents an HTTP request (method, url, headers and body content).</para>
    /// <para>Every <see cref="Request"/> object is effectively immutable. Any modifications produce a new object.</para>
    /// <para>Look at <see cref="RequestUrlBuilder"/> to quickly build request urls with collection initializer syntax.</para>
    /// <para>Look at <see cref="RequestHeadersExtensions"/> to quickly set common headers.</para>
    /// <para>Look at <see cref="RequestContentExtensions"/> to quickly add content to requests.</para>
    /// <para>Look at <see cref="RequestQueryExtensions"/> to quickly add query parameters to requests.</para>
    /// </summary>
    public class Request
    {
        public Request(
            [NotNull] string method,
            [NotNull] Uri url,
            [CanBeNull] Content content = null,
            [CanBeNull] Headers headers = null)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (url == null)
                throw new ArgumentNullException(nameof(url));

            Method = method;
            Url = url;
            Content = content;
            Headers = headers;
        }

        /// <summary>
        /// Returns request method (one of <see cref="RequestMethods"/>).
        /// </summary>
        [NotNull]
        public string Method { get; }

        /// <summary>
        /// Returns request url.
        /// </summary>
        [NotNull]
        public Uri Url { get; }

        /// <summary>
        /// Returns request content or <c>null</c> if there is none.
        /// </summary>
        [CanBeNull]
        public Content Content { get; }

        /// <summary>
        /// Returns request headers or <c>null</c> if there are none.
        /// </summary>
        [CanBeNull]
        public Headers Headers { get; }

        /// <summary>
        /// <para>Returns true if current <see cref="Request"/> instance is valid, or false otherwise.</para>
        /// <para><see cref="Validate"/> method can be used to obtain error messages.</para>
        /// </summary>
        public bool IsValid => !Validate().Any();

        /// <summary>
        /// Produces a new <see cref="Request"/> instance with given url. Current instance is not modified.
        /// </summary>
        /// <returns>A new <see cref="Request"/> object with updated url.</returns>
        [Pure]
        [NotNull]
        public Request WithUrl([NotNull] string url)
        {
            return WithUrl(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Produces a new <see cref="Request"/> instance with given url. Current instance is not modified.
        /// </summary>
        /// <returns>A new <see cref="Request"/> object with updated url.</returns>
        [Pure]
        [NotNull]
        public Request WithUrl([NotNull] Uri url)
        {
            return new Request(Method, url, Content, Headers);
        }

        /// <summary>
        /// Produces a new <see cref="Request"/> instance with given body content. Current instance is not modified.
        /// </summary>
        /// <returns>A new <see cref="Request"/> object with updated content.</returns>
        [Pure]
        [NotNull]
        public Request WithContent([NotNull] Content content)
        {
            return new Request(Method, Url, content, (Headers ?? Headers.Empty).Set(HeaderNames.ContentLength, content.Length));
        }

        /// <summary>
        /// <para>Produces a new <see cref="Request"/> instance where the header with given name will have given value.</para>
        /// <para>See <see cref="Headers"/> class documentation for details.</para>
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value. ToString() is used to obtain string value.</param>
        /// <returns>A new <see cref="Request"/> object with updated headers.</returns>
        [Pure]
        [NotNull]
        public Request WithHeader<T>([NotNull] string name, [NotNull] T value)
        {
            return WithHeader(name, value.ToString());
        }

        /// <summary>
        /// <para>Produces a new <see cref="Request"/> instance where the header with given name will have given value.</para>
        /// <para>See <see cref="Headers"/> class documentation for details.</para>
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        /// <returns>A new <see cref="Request"/> object with updated headers.</returns>
        [Pure]
        [NotNull]
        public Request WithHeader([NotNull] string name, [NotNull] string value)
        {
            return new Request(Method, Url, Content, (Headers ?? Headers.Empty).Set(name, value));
        }

        public override string ToString()
        {
            return ToString(false, false);
        }

        public string ToString(bool includeQuery, bool includeHeaders)
        {
            var builder = new StringBuilder();

            builder.Append(Method);
            builder.Append(" ");

            var urlString = includeQuery 
                ? Url.ToString() 
                : Url.ToStringWithoutQuery();

            builder.Append(urlString);

            if (includeHeaders && Headers != null && Headers.Count > 0)
            {
                builder.AppendLine();
                builder.Append(Headers);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns all validation errors for this <see cref="Request"/> instance. An empty sequence is returned for a valid request.
        /// </summary>
        public IEnumerable<string> Validate()
        {
            if (!RequestMethods.All.Contains(Method))
                yield return $"Request method has unsupported value '{Method}'.";

            if (Url.IsAbsoluteUri)
            {
                var scheme = Url.Scheme;
                if (scheme != Uri.UriSchemeHttp && scheme != Uri.UriSchemeHttps)
                    yield return $"Request url has unsupported scheme '{scheme}'. Only http and https schemes are allowed.";
            }

            if (Content != null && (Method == RequestMethods.Get || Method == RequestMethods.Head))
                yield return $"Sending a body is not allowed with {Method} requests.";
        }

        #region Factory methods

        /// <summary>
        /// Creates a new request with <c>GET</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Get([NotNull] Uri url)
        {
            return new Request(RequestMethods.Get, url);
        }

        /// <summary>
        /// Creates a new request with <c>GET</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Get([NotNull] string url)
        {
            return Get(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>POST</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Post([NotNull] Uri url)
        {
            return new Request(RequestMethods.Post, url);
        }

        /// <summary>
        /// Creates a new request with <c>POST</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Post([NotNull] string url)
        {
            return Post(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>PUT</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Put([NotNull] Uri url)
        {
            return new Request(RequestMethods.Put, url);
        }

        /// <summary>
        /// Creates a new request with <c>PUT</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Put([NotNull] string url)
        {
            return Put(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>HEAD</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Head([NotNull] Uri url)
        {
            return new Request(RequestMethods.Head, url);
        }

        /// <summary>
        /// Creates a new request with <c>HEAD</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Head([NotNull] string url)
        {
            return Head(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>PATCH</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Patch([NotNull] Uri url)
        {
            return new Request(RequestMethods.Patch, url);
        }

        /// <summary>
        /// Creates a new request with <c>PATCH</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Patch([NotNull] string url)
        {
            return Patch(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>DELETE</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Delete([NotNull] Uri url)
        {
            return new Request(RequestMethods.Delete, url);
        }

        /// <summary>
        /// Creates a new request with <c>DELETE</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Delete([NotNull] string url)
        {
            return Delete(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>OPTIONS</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Options([NotNull] Uri url)
        {
            return new Request(RequestMethods.Options, url);
        }

        /// <summary>
        /// Creates a new request with <c>OPTIONS</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Options([NotNull] string url)
        {
            return Options(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a new request with <c>TRACE</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Trace([NotNull] Uri url)
        {
            return new Request(RequestMethods.Trace, url);
        }

        /// <summary>
        /// Creates a new request with <c>TRACE</c> method and given <paramref name="url"/>.
        /// </summary>
        [NotNull]
        public static Request Trace([NotNull] string url)
        {
            return Trace(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        #endregion
    }
}
