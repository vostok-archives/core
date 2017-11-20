namespace Vostok.Clusterclient.Transport.Http
{
    public static class ClusterClientConfigurationExtensions
    {
        /// <summary>
        /// Initialiazes configuration transport with a <see cref="VostokHttpTransport"/>.
        /// </summary>
        public static void SetupVostokHttpTransport(this IClusterClientConfiguration configuration)
        {
            configuration.Transport = new VostokHttpTransport(configuration.Log);
        }
    }
}
