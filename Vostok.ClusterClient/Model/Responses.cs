namespace Vostok.Clusterclient.Model
{
    internal static class Responses
    {
        public static readonly Response Timeout = new Response(ResponseCode.RequestTimeout);
        public static readonly Response Unknown = new Response(ResponseCode.Unknown);
        public static readonly Response UnknownFailure = new Response(ResponseCode.UnknownFailure);
        public static readonly Response Canceled = new Response(ResponseCode.Canceled);
        public static readonly Response Throttled = new Response(ResponseCode.TooManyRequests);
    }
}
