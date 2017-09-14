using Vostok.Clusterclient;
using Vostok.ClusterClient.Transport.Http.Vostok.Http.Client;

namespace Vostok.ClusterClient.Transport.Http.OldConverters
{
    public static class IClusterClientConfigurationExtensions
    {
        /// <summary>
        /// Initialiazes configuration transport with a <see cref="VostokHttpTransport"/> using default <see cref="HttpClientSettings"/>.
        /// </summary>
        public static void SetupKonturHttpTransport(this IClusterClientConfiguration configuration)
        {
            configuration.Transport = new VostokHttpTransport(configuration.Log);
        }

        /// <summary>
        /// Initialiazes configuration transport with a <see cref="VostokHttpTransport"/> using given <paramref name="clientSettings"/>.
        /// </summary>
        public static void SetupKonturHttpTransport(this IClusterClientConfiguration configuration, HttpClientSettings clientSettings)
        {
            configuration.Transport = new VostokHttpTransport(clientSettings, configuration.Log);
        }
    }
}
