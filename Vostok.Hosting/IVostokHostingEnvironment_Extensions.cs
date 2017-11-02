using Vostok.Airlock;

namespace Vostok.Hosting
{
    public static class IVostokHostingEnvironment_Extensions
    {
        public static string GetLoggingRoutingKey(this IVostokHostingEnvironment hostingEnvironment)
        {
            return RoutingKey.TryCreate(hostingEnvironment?.Project, hostingEnvironment?.Environment, hostingEnvironment?.Service, RoutingKey.LogsSuffix);
        }
    }
}