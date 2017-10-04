namespace Vostok.Metrics.Meters.Histograms
{
    public interface IHistogramReservoir : IHistogramMeter
    {
        HistogramSnapshot GetSnapshot();
        void Reset();
    }
}