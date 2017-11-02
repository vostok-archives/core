namespace Vostok.Hosting.Configuration
{
    public interface IApplicationConfigurator
    {
        void OnStart(StartServiceDelegate onStartAsync);
    }
}