using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Misc
{
    public interface IResponseSelector
    {
        /// <summary>
        /// <para>Selects a response which will be returned as a part of <see cref="ClusterResult"/> from given possibilities.</para>
        /// <para>Implementations of this method MUST BE thread-safe.</para>
        /// </summary>
        /// <param name="results">All replica results obtained during request execution.</param>
        /// <returns>Selected response or <c>null</c> if none was selected.</returns>
        [Pure]
        [CanBeNull]
        Response Select([NotNull] IList<ReplicaResult> results);
    }
}
