using System;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// Represents the result of sending request to a replica.
    /// </summary>
    public class ReplicaResult
    {
        public ReplicaResult([NotNull] Uri replica, [NotNull] Response response, ResponseVerdict verdict, TimeSpan time)
        {
            Replica = replica;
            Response = response;
            Verdict = verdict;
            Time = time;
        }

        /// <summary>
        /// Returns replica address.
        /// </summary>
        [NotNull]
        public Uri Replica { get; }

        /// <summary>
        /// Returns replica response (see <see cref="Response"/> for more info).
        /// </summary>
        [NotNull]
        public Response Response { get; }

        /// <summary>
        /// Returns response verdict (see <see cref="ResponseVerdict"/> for more info).
        /// </summary>
        public ResponseVerdict Verdict { get; }

        /// <summary>
        /// Returns request execution time.
        /// </summary>
        public TimeSpan Time { get; }
    }
}
