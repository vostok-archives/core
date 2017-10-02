using System;

namespace Vostok.Commons
{
    public static class HttpClientHostname
    {
        private static readonly string hostname;

        static HttpClientHostname()
        {
            try
            {
                hostname = Environment.MachineName;
            }
            catch (Exception)
            {
                hostname = string.Empty;
            }
        }

        public static string Get()
        {
            return hostname;
        }
    }
}