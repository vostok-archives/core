namespace Vstk.Metrics
{
    public interface IMetricScope
    {
        IMetricEventWriter WriteEvent();
        IMetricEventWriter WriteMetric();
    }
}