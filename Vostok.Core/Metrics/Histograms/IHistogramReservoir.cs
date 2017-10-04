namespace Vostok.Metrics.Histograms
{
    public interface IHistogramReservoir : IHistogramMeter
    {
        HistogramSnapshot GetSnapshot();
        HistogramSnapshot Reset();
    }
}