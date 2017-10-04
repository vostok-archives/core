namespace Vostok.Metrics.Histograms
{
    public interface IHistogramMeter
    {
        void Add(double value);
    }
}