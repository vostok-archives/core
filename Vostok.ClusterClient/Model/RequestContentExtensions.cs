using System;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    public static class RequestContentExtensions
    {
        /// <summary>
        /// Returns a new <see cref="Request"/> instance with content from given <paramref name="buffer"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContent([NotNull] this Request request, [NotNull] byte[] buffer)
        {
            return request.WithContent(new Content(buffer));
        }

        /// <summary>
        /// Returns a new <see cref="Request"/> instance with content from given <paramref name="buffer"/> at given coordinates.
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContent([NotNull] this Request request, [NotNull] byte[] buffer, int offset, int length)
        {
            return request.WithContent(new Content(buffer, offset, length));
        }

        /// <summary>
        /// Returns a new <see cref="Request"/> instance with content from given byte array segment.
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContent([NotNull] this Request request, ArraySegment<byte> content)
        {
            return request.WithContent(new Content(content));
        }

        /// <summary>
        /// Returns a new <see cref="Request"/> instance with content from given string encoded by given <paramref name="encoding"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContent([NotNull] this Request request, [NotNull] string content, [NotNull] Encoding encoding)
        {
            return request.WithContent(new Content(encoding.GetBytes(content)));
        }

        /// <summary>
        /// Returns a new <see cref="Request"/> instance with content from given string encoded by <see cref="UTF8Encoding"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public static Request WithContent([NotNull] this Request request, [NotNull] string content)
        {
            return WithContent(request, content, Encoding.UTF8);
        }
    }
}
