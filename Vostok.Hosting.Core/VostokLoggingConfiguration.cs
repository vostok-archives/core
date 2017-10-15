namespace Vostok.Hosting
{
    public class VostokLoggingConfiguration
    {
        public ILogManager LogManager { get; set; } = new SilentLogManager();

        public string RoutingKey => Airlock.RoutingKey.TryCreate(VostokConfiguration.Project(), VostokConfiguration.Environment(), VostokConfiguration.Service(), Airlock.RoutingKey.LogsSuffix);
    }
}