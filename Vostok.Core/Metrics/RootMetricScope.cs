using System;
using Vostok.Flow;

namespace Vostok.Metrics
{
    public class RootMetricScope : IMetricScope
    {
        private readonly IMetricConfiguration configuration;

        public RootMetricScope(IMetricConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IMetricEventWriter WriteEvent()
        {
            return EnrichWithContext(
                new AirlockMetricEventWriter(configuration.Airlock, configuration.EventRoutingKey));
        }

        public IMetricEventWriter WriteMetric()
        {
            return EnrichWithContext(
                new AirlockMetricEventWriter(configuration.Airlock, configuration.MetricRoutingKey));
        }

        private IMetricEventWriter EnrichWithContext(IMetricEventWriter writer)
        {
            foreach (var pair in Context.Properties.Current)
            {
                if (configuration.ContextFieldsWhitelist.Contains(pair.Key))
                    writer.SetTag(pair.Key, Convert.ToString(pair.Value));
            }
            return writer;
        }
    }
}