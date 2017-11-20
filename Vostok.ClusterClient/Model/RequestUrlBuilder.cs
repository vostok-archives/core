using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Vostok.Clusterclient.Helpers;
using Vostok.Commons.Collections;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// <para>Represents an efficient builder of request urls.</para>
    /// <para>Supports collection initializer syntax:</para>
    /// <list type="bullet">
    /// <item>You can add string or object which are treated as path segments.</item>
    /// <item>You can add key-value pairs which are treated as query parameters.</item>
    /// </list>
    /// </summary>
    /// <example>
    /// <code>
    /// var url = new RequestUrlBuilder
    /// {
    ///     "foo", "bar", "baz",
    ///     { "key", "value" }
    /// }
    /// .Build();
    /// </code>
    /// This creates following url: <c>foo/bar/baz?key=value</c>
    /// </example>
    public class RequestUrlBuilder : IDisposable, IEnumerable
    {
        private static readonly IPool<StringBuilder> builders;

        static RequestUrlBuilder()
        {
            builders = new UnlimitedLazyPool<StringBuilder>(() => new StringBuilder(128));
        }

        private StringBuilder builder;
        private bool hasQueryParameters;
        private Uri result;

        public RequestUrlBuilder([NotNull] string initialUrl = "")
        {
            if (initialUrl == null)
                throw new ArgumentNullException(nameof(initialUrl));

            builder = builders.Acquire();
            builder.Clear();
            builder.Append(initialUrl);

            hasQueryParameters = initialUrl.IndexOf("?", StringComparison.Ordinal) >= 0;
        }

        public bool IsDisposed => builder == null;

        public void Dispose()
        {
            var oldBuilder = Interlocked.Exchange(ref builder, null);
            if (oldBuilder == null)
                return;

            builders.Release(oldBuilder);
        }

        /// <summary>
        /// Builds a final <see cref="Uri"/> and disposes this builder instance. No further actions are possible after <see cref="Build"/> call.
        /// </summary>
        [NotNull]
        public Uri Build()
        {
            if (result != null)
                return result;

            using (this)
                return result = new Uri(builder.ToString(), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// <para>Appends a new path <paramref name="segment"/>. <see cref="object.ToString"/> is called on <paramref name="segment"/>.</para>
        /// <para>Note that it's not possible to append to path when url already has some query parameters.</para>
        /// </summary>
        [NotNull]
        public RequestUrlBuilder AppendToPath<T>([CanBeNull] T segment)
        {
            return AppendToPath(FormatValue(segment));
        }

        /// <summary>
        /// <para>Appends a new path <paramref name="segment"/>.</para>
        /// <para>Note that it's not possible to append to path when url already has some query parameters.</para>
        /// </summary>
        [NotNull]
        public RequestUrlBuilder AppendToPath([CanBeNull] string segment)
        {
            EnsureNotDisposed();
            EnsureQueryNotStarted();

            if (string.IsNullOrEmpty(segment))
                return this;

            // ReSharper disable once PossibleNullReferenceException
            if (segment.StartsWith("/"))
            {
                if (builder.Length > 0 && builder[builder.Length - 1] == '/')
                    builder.Remove(builder.Length - 1, 1);
            }
            else
            {
                if (builder.Length > 0 && builder[builder.Length - 1] != '/')
                    builder.Append('/');
            }

            builder.Append(segment);

            return this;
        }

        /// <summary>
        /// <para>Appends a new query parameter with given <paramref name="key"/> and <paramref name="value"/>. <see cref="object.ToString"/> is called on <paramref name="value"/>.</para>
        /// <para><paramref name="key"/> and <paramref name="value"/> are encoded using percent-encoding.</para>
        /// </summary>
        [NotNull]
        public RequestUrlBuilder AppendToQuery<T>([CanBeNull] string key, [CanBeNull] T value)
        {
            return AppendToQuery(key, FormatValue(value));
        }

        /// <summary>
        /// <para>Appends a new query parameter with given <paramref name="key"/> and <paramref name="value"/>.</para>
        /// <para><paramref name="key"/> and <paramref name="value"/> are encoded using percent-encoding.</para>
        /// </summary>
        [NotNull]
        public RequestUrlBuilder AppendToQuery([CanBeNull] string key, [CanBeNull] string value)
        {
            EnsureNotDisposed();

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return this;

            if (hasQueryParameters)
            {
                builder.Append('&');
            }
            else
            {
                builder.Append('?');
                hasQueryParameters = true;
            }

            builder.Append(UrlEncodingHelper.UrlEncode(key, Encoding.UTF8));
            builder.Append('=');
            builder.Append(UrlEncodingHelper.UrlEncode(value, Encoding.UTF8));

            return this;
        }

        /// <summary>
        /// Same as <see cref="AppendToPath"/>. Needed for collection initializer syntax.
        /// </summary>
        public void Add([CanBeNull] string segment)
        {
            AppendToPath(segment);
        }

        /// <summary>
        /// Same as <see cref="AppendToPath{T}"/>. Needed for collection initializer syntax.
        /// </summary>
        public void Add<T>([CanBeNull] T segment)
        {
            AppendToPath(segment);
        }

        /// <summary>
        /// Same as <see cref="AppendToQuery"/>. Needed for collection initializer syntax.
        /// </summary>
        public void Add([CanBeNull] string key, [CanBeNull] string value)
        {
            AppendToQuery(key, value);
        }

        /// <summary>
        /// Same as <see cref="AppendToQuery{T}"/>. Needed for collection initializer syntax.
        /// </summary>
        public void Add<T>([CanBeNull] string key, [CanBeNull] T value)
        {
            AppendToQuery(key, value);
        }

        protected void EnsureNotDisposed()
        {
            if (builder == null)
                throw new ObjectDisposedException(nameof(builder), "Can not reuse a builder which already built an url.");
        }

        protected void EnsureQueryNotStarted()
        {
            if (hasQueryParameters)
                throw new InvalidOperationException("Can not append to path after appending query parameters.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatValue<T>(T value)
        {
            if (!typeof (T).IsValueType && Equals(value, null))
                return null;

            return value.ToString();
        }
    }
}
