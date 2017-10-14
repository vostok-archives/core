namespace Vostok.Clusterclient.Strategies.TimeoutProviders
{
    /// <summary>
    /// Represents a behaviour in case when provided timeout values for <see cref="FixedTimeoutsProvider"/> or <see cref="AdHocTimeoutsProvider"/> are exhausted.
    /// </summary>
    public enum TailTimeoutBehaviour
    {
        /// <summary>
        /// Repeat last timeout for all subsequent requests.
        /// </summary>
        RepeatLastValue = 0,

        /// <summary>
        /// Use whole remaining time budget for all subsequent requests.
        /// </summary>
        UseRemainingBudget = 1
    }
}
