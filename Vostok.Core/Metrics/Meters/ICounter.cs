namespace Vostok.Metrics.Meters
{
    public interface ICounter
    {
        void Add(long value = 1);
    }
}