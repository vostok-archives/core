using System.Text;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// Represents an HTTP response from server.
    /// </summary>
    public class Response
    {
        private readonly Content content;
        private readonly Headers headers;

        public Response(ResponseCode code, [CanBeNull] Content content = null, [CanBeNull] Headers headers = null)
        {
            Code = code;
            this.content = content;
            this.headers = headers;
        }

        /// <summary>
        /// Returns response code.
        /// </summary>
        public ResponseCode Code { get; }

        /// <summary>
        /// Returns response body content or an empty content if there is none.
        /// </summary>
        [NotNull]
        public Content Content => content ?? Content.Empty;

        /// <summary>
        /// Returns response headers or empty headers if there are none.
        /// </summary>
        [NotNull]
        public Headers Headers => headers ?? Headers.Empty;

        /// <summary>
        /// Returns <c>true</c> if <see cref="Code"/> belongs to 2xx range (see <see cref="ResponseCodeExtensions.IsSuccessful"/>), or false otherwise.
        /// </summary>
        public bool IsSuccessful => Code.IsSuccessful();

        /// <summary>
        /// Throws a <see cref="ClusterClientException"/> if <see cref="Code"/> doesn't belong to 2xx range (see <see cref="ResponseCodeExtensions.IsSuccessful"/>)
        /// </summary>
        public Response EnsureSuccessStatusCode()
        {
            if (!IsSuccessful)
                throw new ClusterClientException($"Response status code '{Code}' indicates unsuccessful outcome.");

            return this;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool includeHeaders)
        {
            var builder = new StringBuilder();

            builder.Append((int) Code);
            builder.Append(" ");
            builder.Append(Code);

            if (includeHeaders && headers != null && headers.Count > 0)
            {
                builder.AppendLine();
                builder.Append(headers);
            }

            return builder.ToString();
        }
    }
}
