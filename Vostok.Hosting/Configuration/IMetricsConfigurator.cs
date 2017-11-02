namespace Vostok.Hosting.Configuration
{
    public interface IMetricsConfigurator
    {
        void AddContextFieldswhitelist(params string[] fields);
    }
}