namespace Vostok.Hosting.Configuration
{
    public interface ITracingConfigurator
    {
        void AddContextFieldswhitelist(params string[] fields);
        void AddInheritedFieldswhitelist(params string[] fields);
    }
}