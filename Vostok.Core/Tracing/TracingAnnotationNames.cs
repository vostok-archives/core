namespace Vostok.Tracing
{
    public static class TracingAnnotationNames
    {
        public const string OperationName = "operationName";
        public const string ServiceName = "serviceName";
        public const string Component = "component";
        public const string Kind = "kind";

        public const string ClusterStrategy = "cluster.strategy";
        public const string ClusterStatus = "cluster.status";

        public const string HttpUrl = "http.url";
        public const string HttpCode = "http.code";
        public const string HttpMethod = "http.method";
        public const string HttpRequestContentLength = "http.requestContentLength";
        public const string HttpResponseContentLength = "http.responseContentLength";
    }
}