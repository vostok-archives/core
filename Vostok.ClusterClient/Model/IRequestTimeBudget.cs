using System;

namespace Vostok.Clusterclient.Model
{
    /// <summary>
    /// Represents a time budget allocated for request execution.
    /// </summary>
    public interface IRequestTimeBudget
    {
        /// <summary>
        /// Returns a total amount of time allocated for request.
        /// </summary>
        TimeSpan Total { get; }

        /// <summary>
        /// Returns elapsed amount of time since the beginning of request execution.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Returns remaining amount of time for request execution or <see cref="TimeSpan.Zero"/> if budget has already expired.
        /// </summary>
        TimeSpan Remaining { get; }

        /// <summary>
        /// Returns <c>true</c> if budget has already expired, or <c>false</c> otherwise.
        /// </summary>
        bool HasExpired { get; }
    }
}