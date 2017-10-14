using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// Represent the final result of sending request to a cluster of replicas.
    /// </summary>
    public class ClusterResult
    {
        internal static ClusterResult TimeExpired(Request request)
        {
            return new ClusterResult(ClusterResultStatus.TimeExpired, new ReplicaResult[] {}, null, request);
        }

        internal static ClusterResult ReplicasNotFound(Request request)
        {
            return new ClusterResult(ClusterResultStatus.ReplicasNotFound, new ReplicaResult[] {}, null, request);
        }

        internal static ClusterResult IncorrectArguments(Request request)
        {
            return new ClusterResult(ClusterResultStatus.IncorrectArguments, new ReplicaResult[] {}, null, request);
        }

        internal static ClusterResult UnexpectedException(Request request)
        {
            return new ClusterResult(ClusterResultStatus.UnexpectedException, new ReplicaResult[] {}, null, request);
        }

        internal static ClusterResult Canceled(Request request)
        {
            return new ClusterResult(ClusterResultStatus.Canceled, new ReplicaResult[] { }, null, request);
        }

        internal static ClusterResult Throttled(Request request)
        {
            return new ClusterResult(ClusterResultStatus.Throttled, new ReplicaResult[] { }, null, request);
        }

        private readonly Response selectedResponse;

        public ClusterResult(
            ClusterResultStatus status,
            [NotNull] IList<ReplicaResult> replicaResults,
            [CanBeNull] Response selectedResponse,
            [NotNull] Request request)
        {
            this.selectedResponse = selectedResponse;

            Status = status;
            Request = request;
            ReplicaResults = replicaResults;
        }

        /// <summary>
        /// Returns result status. <see cref="ClusterResultStatus.Success"/> value indicates that everything's good.
        /// </summary>
        public ClusterResultStatus Status { get; }

        /// <summary>
        /// <para>Returns the results of replica requests made during request execution.</para>
        /// <para>
        /// Returned list may contain results whose responses have <see cref="ResponseCode.Unknown"/> code.
        /// This usually indicates that request execution has stopped before receiving responses from these replicas.
        /// This can occur when using a <see cref="Strategies.ParallelRequestStrategy"/> or <see cref="Strategies.ForkingRequestStrategy"/>.
        /// </para>
        /// </summary>
        [NotNull]
        public IList<ReplicaResult> ReplicaResults { get; }

        /// <summary>
        /// <para>Returns the final selected response.</para>
        /// <para>By default this property returns a response selected by <see cref="Misc.IResponseSelector"/> implementation.</para>
        /// <para>If no response was received or explicitly selected, this property returns a generated response:</para>
        /// <list type="bullet">
        /// <item><see cref="ClusterResultStatus.TimeExpired"/> --> <see cref="ResponseCode.RequestTimeout"/></item>
        /// <item><see cref="ClusterResultStatus.UnexpectedException"/> --> <see cref="ResponseCode.UnknownFailure"/></item>
        /// <item>any other status --> <see cref="ResponseCode.Unknown"/></item>
        /// </list>
        /// </summary>
        [NotNull]
        public Response Response => selectedResponse ?? GetResponseByStatus();

        /// <summary>
        /// <para>Returns the address of replica which returned final selected <see cref="Response"/>.</para>
        /// <para>May return <c>null</c> if such replica cannot be chosen.</para>
        /// </summary>
        [CanBeNull]
        public Uri Replica => ReplicaResults.FirstOrDefault(r => ReferenceEquals(r.Response, Response))?.Replica;

        /// <summary>
        /// Returns a request which has been sent with this result.
        /// </summary>
        [NotNull]
        public Request Request { get; }

        private Response GetResponseByStatus()
        {
            switch (Status)
            {
                case ClusterResultStatus.TimeExpired:
                    return Responses.Timeout;

                case ClusterResultStatus.UnexpectedException:
                    return Responses.UnknownFailure;

                case ClusterResultStatus.Canceled:
                    return Responses.Canceled;

                case ClusterResultStatus.Throttled:
                    return Responses.Throttled;

                default:
                    return Responses.Unknown;
            }
        }
    }
}
