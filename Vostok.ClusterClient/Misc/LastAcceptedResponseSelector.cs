using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Misc
{
    /// <summary>
    /// Represents a response selector which works using following priority system:
    /// <list type="number">
    /// <item>If there are no results at all, it returns <c>null</c>.</item>
    /// <item>If there are any results with <see cref="ResponseVerdict.Accept"/> verdict, it returns the last of them.</item>
    /// <item>If there are any results with response code other than <see cref="ResponseCode.Unknown"/>, it returns the last of them.</item>
    /// <item>As a last resort, it just returns response of last result in the list.</item>
    /// </list>
    /// </summary>
    public class LastAcceptedResponseSelector : IResponseSelector
    {
        public Response Select(IList<ReplicaResult> results)
        {
            return GetLastAcceptedResponse(results) ?? GetLastKnownResponse(results) ?? GetLastResponse(results);
        }

        private static Response GetLastAcceptedResponse(IList<ReplicaResult> results)
        {
            return results.LastOrDefault(result => result.Verdict == ResponseVerdict.Accept)?.Response;
        }

        private static Response GetLastKnownResponse(IList<ReplicaResult> results)
        {
            return results.LastOrDefault(result => result.Response.Code != ResponseCode.Unknown)?.Response;
        }

        private static Response GetLastResponse(IList<ReplicaResult> results)
        {
            return results.LastOrDefault()?.Response;
        }
    }
}
