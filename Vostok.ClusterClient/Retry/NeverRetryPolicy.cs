using System.Collections.Generic;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Retry
{
    /// <summary>
    /// Represents a policy which never chooses to retry cluster communication.
    /// </summary>
    public class NeverRetryPolicy : IRetryPolicy
    {
        public bool NeedToRetry(IList<ReplicaResult> results)
        {
            return false;
        }
    }
}
