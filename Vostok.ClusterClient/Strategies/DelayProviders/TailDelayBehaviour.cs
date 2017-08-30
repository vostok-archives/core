namespace Vostok.Clusterclient.Strategies.DelayProviders
{
    /// <summary>
    /// Represents a behaviour in case when provided delay values for <see cref="FixedDelaysProvider"/> or <see cref="AdHocDelaysProvider"/> are exhausted.
    /// </summary>
    public enum TailDelayBehaviour
    {
        /// <summary>
        /// Repeat last delay for all subsequent requests.
        /// </summary>
        RepeatLastValue = 0,

        /// <summary>
        /// Repeat all delays in a cyclic fashion.
        /// </summary>
        RepeatAllValues = 1,

        /// <summary>
        /// Stop issuing any delays when values are exhausted.
        /// </summary>
        StopIssuingDelays = 2
    }
}
