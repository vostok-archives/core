namespace Vostok.Commons.Model
{
    public enum RequestPriority
    {
        /// <summary>
        /// Request failure does not lead to significant user-visible harm (e.g. requests from background batch jobs).
        /// </summary>
        Sheddable = 0,

        /// <summary>
        /// Request is not accurately described by any of <see cref="Sheddable"/> and <see cref="Critical"/> priorities.
        /// </summary>
        Ordinary = 1,

        /// <summary>
        /// The request plays critical role in user-facing task and would cause considerable harm if not fulfilled.
        /// </summary>
        Critical = 2
    }
}