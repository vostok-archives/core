using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which rejects responses with <see cref="ResponseCode.Unknown"/> and <see cref="ResponseCode.UnknownFailure"/> codes.
    /// </summary>
    public class RejectUnknownErrorsCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            switch (response.Code)
            {
                case ResponseCode.Unknown:
                case ResponseCode.UnknownFailure:
                    return ResponseVerdict.Reject;

                default:
                    return ResponseVerdict.DontKnow;
            }
        }
    }
}
