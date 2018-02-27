namespace Vstk.Tracing
{
    public static class TracingAnnotationNames
    {
        public const string Operation = "operation";
        public const string Service = "service";
        public const string Component = "component";
        public const string Kind = "kind";
        public const string Host = "host";

        public const string ClusterStrategy = "cluster.strategy";
        public const string ClusterStatus = "cluster.status";

        public const string HttpUrl = "http.url";
        public const string HttpCode = "http.code";
        public const string HttpMethod = "http.method";
        public const string HttpRequestContentLength = "http.requestContentLength";
        public const string HttpResponseContentLength = "http.responseContentLength";
    }
}