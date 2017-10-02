using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transport.Http
{
    internal static class ResponseFactory
    {
        public static Response BuildSuccessResponse(HttpWebRequestState state)
        {
            return BuildResponse((ResponseCode) (int) state.Response.StatusCode, state);
        }

        public static Response BuildFailureResponse(HttpActionStatus status, HttpWebRequestState state)
        {
            switch (status)
            {
                case HttpActionStatus.SendFailure:
                    return BuildResponse(ResponseCode.SendFailure, state);

                case HttpActionStatus.ReceiveFailure:
                    return BuildResponse(ResponseCode.ReceiveFailure, state);

                case HttpActionStatus.Timeout:
                    return BuildResponse(ResponseCode.RequestTimeout, state);

                case HttpActionStatus.RequestCanceled:
                    return BuildResponse(ResponseCode.Canceled, state);

                default:
                    return BuildResponse(ResponseCode.UnknownFailure, state);
            }
        }

        public static Response BuildResponse(ResponseCode code, HttpWebRequestState state)
        {
            return new Response(code, CreateResponseContent(state), CreateResponseHeaders(state));
        }

        private static Content CreateResponseContent(HttpWebRequestState state)
        {
            if (state.BodyBuffer != null)
                return new Content(state.BodyBuffer);

            if (state.BodyStream != null)
                return new Content(state.BodyStream.GetBuffer(), 0, (int)state.BodyStream.Position);

            return null;
        }

        private static Headers CreateResponseHeaders(HttpWebRequestState state)
        {
            var headers = Headers.Empty;

            if (state.Response == null)
                return headers;

            foreach (var key in state.Response.Headers.AllKeys)
            {
                headers = headers.Set(key, state.Response.Headers[key]);
            }

            return headers;
        }
    }
}