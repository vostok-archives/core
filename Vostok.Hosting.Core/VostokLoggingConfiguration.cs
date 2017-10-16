namespace Vostok.Hosting
{
    public class VostokLoggingConfiguration
    {
        public string RoutingKey => Airlock.RoutingKey.TryCreate(VostokConfiguration.Project(), VostokConfiguration.Environment(), VostokConfiguration.Service(), Airlock.RoutingKey.LogsSuffix);
    }
}