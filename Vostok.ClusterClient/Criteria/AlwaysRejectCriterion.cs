using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which rejects any response.
    /// </summary>
    public class AlwaysRejectCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            return ResponseVerdict.Reject;
        }
    }
}
