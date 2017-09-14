using System.Diagnostics;

namespace Vostok.ClusterClient.Transport.Http.Vostok.Http.Client
{
    //todo (@ezsilmar)
    //this class is in namespace Kontur.Net.Http
    //because it was moved from http module and some usages remains
    public static class HttpClientIdentity
    {
        static HttpClientIdentity()
        {
            Identity = GetIdentityFromHostingEnvironmentOrNull();
            if (!IsValidIdentity(Identity))
            {
                Identity = GetProcessNameOrNull();
            }
            if (!IsValidIdentity(Identity))
            {
                Identity = "Unknown";
            }
            Identity = Identity.Replace('/', '.');
        }

        private static bool IsValidIdentity(string identity)
        {
            return !string.IsNullOrEmpty(identity) && !string.IsNullOrWhiteSpace(identity);
        }

        private static string GetIdentityFromHostingEnvironmentOrNull()
        {
            try
            {
                if (!HostingEnvironment.IsHosted)
                {
                    return null;
                }

                var vPath = HostingEnvironment.ApplicationVirtualPath;
                var siteName = HostingEnvironment.SiteName;
                if (vPath == null || vPath == "/")
                {
                    return siteName;
                }
                return siteName + vPath;
            }
            catch
            {
                return null;
            }
        }

        private static string GetProcessNameOrNull()
        {
            try
            {
                return Process.GetCurrentProcess().ProcessName;
            }
            catch
            {
                return null;
            }
        }

        public static string Get()
        {
            return Identity;
        }

        private static readonly string Identity;
    }
}