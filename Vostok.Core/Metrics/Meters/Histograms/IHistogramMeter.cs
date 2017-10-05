namespace Vostok.Metrics.Meters.Histograms
{
    public interface IHistogramMeter
    {
        void Add(double value);
    }
}