using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Criteria
{
    /// <summary>
    /// <para>Represents a criterion which rejects responses with some of the server error codes (see <see cref="ResponseCodeExtensions.IsServerError"/>).</para>
    /// <para>The exceptions are <see cref="ResponseCode.NotImplemented"/> and <see cref="ResponseCode.HttpVersionNotSupported"/> codes which are not rejected.</para>
    /// </summary>
    public class RejectServerErrorsCriterion : IResponseCriterion
    {
        public ResponseVerdict Decide(Response response)
        {
            switch (response.Code)
            {
                case ResponseCode.InternalServerError:
                case ResponseCode.BadGateway:
                case ResponseCode.ServiceUnavailable:
                case ResponseCode.ProxyTimeout:
                    return ResponseVerdict.Reject;

                default:
                    return ResponseVerdict.DontKnow;
            }
        }
    }
}
