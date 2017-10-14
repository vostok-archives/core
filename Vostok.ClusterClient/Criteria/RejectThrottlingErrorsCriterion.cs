using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which rejects responses with <see cref="ResponseCode.TooManyRequests"/> code.
    /// </summary>
    public class RejectThrottlingErrorsCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            return response.Code == ResponseCode.TooManyRequests ? ResponseVerdict.Reject : ResponseVerdict.DontKnow;
        }
    }
}