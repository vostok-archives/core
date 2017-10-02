namespace Vostok.Metrics
{
    public interface IMetricScope
    {
        IMetricEventWriter WriteEvent();
        IMetricEventWriter WriteMetric();
    }
}