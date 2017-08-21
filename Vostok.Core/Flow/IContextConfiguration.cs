namespace Vostok.Flow
{
    public interface IContextConfiguration
    {
        bool IsDistributedProperty(string key);

        void AddDistributedProperty(string key);
    }
}