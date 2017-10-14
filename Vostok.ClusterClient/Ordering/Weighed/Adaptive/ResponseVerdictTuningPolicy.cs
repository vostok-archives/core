using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    /// <summary>
    /// <para>Represents a tuning policy which selects action based on replica's response verdict:</para>
    /// <list type="bullet">
    /// <item><see cref="ResponseVerdict.Accept"/> verdict leads to <see cref="AdaptiveHealthAction.Increase"/> of replica health.</item>
    /// <item><see cref="ResponseVerdict.Reject"/> verdict leads to <see cref="AdaptiveHealthAction.Decrease"/> of replica health.</item>
    /// </list>
    /// </summary>
    public class ResponseVerdictTuningPolicy : IAdaptiveHealthTuningPolicy
    {
        public AdaptiveHealthAction SelectAction(ReplicaResult result)
        {
            switch (result.Verdict)
            {
                case ResponseVerdict.Accept:
                    return AdaptiveHealthAction.Increase;

                case ResponseVerdict.Reject:
                    return AdaptiveHealthAction.Decrease;

                default:
                    return AdaptiveHealthAction.DontTouch;
            }
        }
    }
}
