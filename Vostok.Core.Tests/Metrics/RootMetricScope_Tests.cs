using NSubstitute;
using NUnit.Framework;

namespace Vostok.Metrics
{
    public class RootMetricScope_Tests
    {
        private IMetricConfiguration metricConfiguration;
        private IMetricEventReporter eventReporter;
        private RootMetricScope scope;

        [SetUp]
        public void SetUp()
        {
            eventReporter = Substitute.For<IMetricEventReporter>();
            metricConfiguration = Substitute.For<IMetricConfiguration>();
            metricConfiguration.Reporter.Returns(eventReporter);
            scope = new RootMetricScope(metricConfiguration);
        }
        
        [Test]
        public void Write_event_should_record_event()
        {
            scope.WriteEvent().Commit();
            eventReporter.Received(1).SendEvent(Arg.Any<MetricEvent>());
        }

        [Test]
        public void Write_metric_should_record_event()
        {
            scope.WriteMetric().Commit();
            eventReporter.Received(1).SendMetric(Arg.Any<MetricEvent>());
        }
    }
}