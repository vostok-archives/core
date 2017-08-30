using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// Represents a criterion which accepts any response with <see cref="HeaderNames.XKonturDontRetry"/> header.
    /// </summary>
    public class AcceptNonRetriableCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            return response.Headers[HeaderNames.XKonturDontRetry] != null ? ResponseVerdict.Accept : ResponseVerdict.DontKnow;
        }
    }
}