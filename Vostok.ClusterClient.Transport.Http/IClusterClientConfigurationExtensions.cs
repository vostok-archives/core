namespace Vostok.Clusterclient.Transport.Http
{
    public static class IClusterClientConfigurationExtensions
    {
        /// <summary>
        /// Initialiazes configuration transport with a <see cref="VostokHttpTransport"/> using default <see cref="VostokHttpTransportSettings"/>.
        /// </summary>
        public static void SetupVostokHttpTransport(this IClusterClientConfiguration configuration)
        {
            configuration.Transport = new VostokHttpTransport(configuration.Log);
        }
    }
}
