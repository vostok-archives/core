namespace Vostok.Hosting
{
    public static class VostokConfigurationDefaults
    {
        public const string AirlockSection = "airlock";
        public const string TracingSection = "tracing";
        public const string MetricsSection = "metrics";
        public const string ProjectKey = "project";
        public const string ServiceKey = "service";
        public const string EnvironmentKey = "environment";
        public const string HostKey = "host";
        public const string AirlockParallelismKey = "parallelism";
        public const int DefaultAirlockParallelism = 1;
    }
}