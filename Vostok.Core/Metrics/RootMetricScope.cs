using Vostok.Commons.Collections;

namespace Vostok.Metrics
{
    public class RootMetricScope : IMetricScope
    {
        private readonly IMetricConfiguration configuration;
        private readonly UnlimitedLazyPool<MetricEventWriter> eventWriterPool;
        private readonly UnlimitedLazyPool<MetricEventWriter> metricWriterPool;

        public RootMetricScope(IMetricConfiguration configuration)
        {
            this.configuration = configuration;
            this.eventWriterPool = new UnlimitedLazyPool<MetricEventWriter>(
                () => new MetricEventWriter(
                    eventWriterPool,
                    configuration,
                    metricEvent => configuration.Reporter?.SendEvent(metricEvent)));
            this.metricWriterPool = new UnlimitedLazyPool<MetricEventWriter>(
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