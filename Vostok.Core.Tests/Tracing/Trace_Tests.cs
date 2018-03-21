using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Vstk.Tracing
{
    public class Trace_Tests
    {
        private ITraceReporter traceReporter;

        [SetUp]
        public void SetUp()
        {
            traceReporter = Substitute.For<ITraceReporter>();
            Trace.Configuration.Reporter = traceReporter;
            Trace.Configuration.InheritedFieldsWhitelist.Clear();
        }

        [Test]
        public void BeginSpan_should_inherit_custom_field_from_whitelist()
        {
            const string customKey = "customKey";
            const string customValue = "customValue";

            Trace.Configuration.InheritedFieldsWhitelist.Add(customKey);

            traceReporter.SendSpan(
                Arg.Do<Span>(
                    x =>
                    {
                        x.Annotations.Keys.Should().Contain(customKey);
                        x.Annotations[customKey].Should().Contain(customValue);
                    }));

            using (var span = Trace.BeginSpan())
            {
                span.SetAnnotation(customKey, customValue);
                using (Trace.BeginSpan())
                {
                }
            }

            traceReporter.Received(2).SendSpan(Arg.Any<Span>());
        }
    }
}
