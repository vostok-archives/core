using System;
using JetBrains.Annotations;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// <para>Represents a strategy which determines cluster communication attempts count and delays between attempts.</para>
    /// <para>Note that this retry mechanism applies to whole cluster communication attempts (it only gets used when all replicas have failed to produce an <see cref="Model.ResponseVerdict.Accept"/>ed response).</para>
    /// <para>Such a retry mechanism is suitable for small clusters which can be fully temporarily unavailable during normal operation (such as leadership ensembles).</para>
    /// </summary>
    public interface IRetryStrategy
    {
        /// <summary>
        /// Returns maximum attempts count. Values less than 1 are ignored.
        /// </summary>
        int AttemptsCount { get; }

        /// <summary>
        /// <para>Returns a retry delay before next attempt based on currently used attempts count. Values less or equal to zero are ignored.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        [Pure]
        TimeSpan GetRetryDelay(int attemptsUsed);
    }
}
