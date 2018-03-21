using Vstk.Commons.Collections;

namespace Vstk.Metrics
{
    public class RootMetricScope : IMetricScope
    {
        private readonly UnlimitedLazyPool<MetricEventWriter> eventWriterPool;
        private readonly UnlimitedLazyPool<MetricEventWriter> metricWriterPool;

        public RootMetricScope(IMetricConfiguration configuration)
        {
            eventWriterPool = new UnlimitedLazyPool<MetricEventWriter>(
                () => new MetricEventWriter(
                    eventWriterPool,
                    configuration,
                    metricEvent => configuration.Reporter?.SendEvent(metricEvent)));
            metricWriterPool = new UnlimitedLazyPool<MetricEventWriter>(
                () => new MetricEventWriter(
                    metricWriterPool,
                    configuration,
                    metricEvent => configuration.Reporter?.SendMetric(metricEvent)));
        }

        public IMetricEventWriter WriteEvent()
        {
            var writer = eventWriterPool.Acquire();
            writer.Initialize();
            return writer;
        }

        public IMetricEventWriter WriteMetric()
        {
            var writer = metricWriterPool.Acquire();
            writer.Initialize();
            return writer;
        }
    }
}