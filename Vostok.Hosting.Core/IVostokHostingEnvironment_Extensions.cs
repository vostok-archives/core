using System;
using Vostok.Airlock;

namespace Vostok.Hosting
{
    public static class VostokHostingEnvironment_Extensions
    {
        public static string GetLoggingRoutingKey(this IVostokHostingEnvironment hostingEnvironment)
        {
            return RoutingKey.TryCreate(hostingEnvironment?.Project, hostingEnvironment?.Environment, hostingEnvironment?.Service, RoutingKey.LogsSuffix);
        }

        public static bool IsEnvironment(this IVostokHostingEnvironment hostingEnvironment, string environmentName)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));
            return string.Equals(hostingEnvironment.Environment, environmentName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsProduction(this IVostokHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));
            return hostingEnvironment.IsEnvironment(VostokEnvironmentNames.Production);
        }

        public static bool IsDevelopment(this IVostokHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));
            return hostingEnvironment.IsEnvironment(VostokEnvironmentNames.Development);
        }

        public static bool IsStaging(this IVostokHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));
            return hostingEnvironment.IsEnvironment(VostokEnvironmentNames.Staging);
        }
    }
}