namespace Vostok.Airlock
{
    public interface IAirlock
    {
        void Push<T>(string category, T item);
    }
}
