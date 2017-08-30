namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// Represent the final status of sending request to a cluster of replicas.
    /// </summary>
    public enum ClusterResultStatus
    {
        /// <summary>
        /// At least one response with <see cref="ResponseVerdict.Accept"/> verdict was obtained.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Request time budget has expired before any <see cref="ResponseVerdict.Accept"/>ed response was obtained.
        /// </summary>
        TimeExpired = 1,

        /// <summary>
        /// No replicas were resolved to send request to.
        /// </summary>
        ReplicasNotFound = 2,

        /// <summary>
        /// None of the replicas returned an <see cref="ResponseVerdict.Accept"/>ed response.
        /// </summary>
        ReplicasExhausted = 3,

        /// <summary>
        /// One of provided parameters (<see cref="Request"/>, timeout) was invalid.
        /// </summary>
        IncorrectArguments = 4,

        /// <summary>
        /// An unexpected exception has arised during request execution.
        /// </summary>
        UnexpectedException = 5,

        /// <summary>
        /// Request execution was canceled via <see cref="System.Threading.CancellationToken"/>.
        /// </summary>
        Canceled = 6,

        /// <summary>
        /// Request was rejected locally without cluster communication due to throttling mechanism.
        /// </summary>
        Throttled = 7
    }
}
