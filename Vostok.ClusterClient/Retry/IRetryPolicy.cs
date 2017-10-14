using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// <para>Represents a policy which determines whether it's needed to retry cluster communication based on current attempt results.</para>
    /// <para>Note that this retry mechanism applies to whole cluster communication attempts (it only gets used when all replicas have failed to produce an <see cref="ResponseVerdict.Accept"/>ed response).</para>
    /// <para>Such a retry mechanism is suitable for small clusters which can be fully temporarily unavailable during normal operation (such as leadership ensembles).</para>
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// <para>Returns true if current <paramref name="results"/> indicate the need to retry cluster communication, or false otherwise.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        [Pure]
        bool NeedToRetry([NotNull] [ItemNotNull] IList<ReplicaResult> results);
    }
}
