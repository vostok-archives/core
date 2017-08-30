namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    /// <summary>
    /// Represents an action to be taken on replica health value.
    /// </summary>
    public enum AdaptiveHealthAction
    {
        /// <summary>
        /// Health should not be modified.
        /// </summary>
        DontTouch = 0,

        /// <summary>
        /// Health should be increased.
        /// </summary>
        Increase = 1,

        /// <summary>
        /// Health should be decreased.
        /// </summary>
        Decrease = 2
    }
}
