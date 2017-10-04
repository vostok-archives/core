using System;

namespace Vostok.Commons
{
    public static class HostnameProvider
    {
        private static readonly string hostname;

        static HostnameProvider()
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