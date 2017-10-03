namespace Vostok.Metrics
{
    public interface ICounter
    {
        void Add(long value = 1);
    }
}