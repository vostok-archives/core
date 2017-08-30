using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which accepts any response.
    /// </summary>
    public class AlwaysAcceptCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            return ResponseVerdict.Accept;
        }
    }
}
