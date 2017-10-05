namespace Vostok.Metrics.Meters.Histograms
{
    public interface IReservoirHistogramMeter : IHistogramMeter
    {
        ReservoirHistogramSnapshot GetSnapshot();
        void Reset();
    }
}