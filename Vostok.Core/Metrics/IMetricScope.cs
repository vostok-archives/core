namespace Vostok.Metrics
{
    public interface IMetricScope
    {
        IMetricEventWriter WriteEvent();
        IMetricEventWriter WriteMetric();
    }

    public class MetricScope : IMetricScope
    {
        
        public IMetricEventWriter WriteEvent()
        {
            throw new System.NotImplementedException();
        }

        public IMetricEventWriter WriteMetric()
        {
            throw new System.NotImplementedException();
        }
    }
}