using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which rejects redirection (3xx) responses except for <see cref="ResponseCode.NotModified"/> code.
    /// </summary>
    public class RejectRedirectionsCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            if (response.Code.IsRedirection() && response.Code != ResponseCode.NotModified)
                return ResponseVerdict.Reject;

            return ResponseVerdict.DontKnow;
        }
    }
}