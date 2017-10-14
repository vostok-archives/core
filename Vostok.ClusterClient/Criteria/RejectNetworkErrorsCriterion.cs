using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which rejects responses that indicate network errors (see <see cref="ResponseCodeExtensions.IsNetworkError"/>).
    /// </summary>
    public class RejectNetworkErrorsCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            return response.Code.IsNetworkError() ? ResponseVerdict.Reject : ResponseVerdict.DontKnow;
        }
    }
}
