using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Airlock;

namespace Vostok.Tracing
{
    public class Trace_Tests
    {
        private IAirlock airlock;

        [SetUp]
        public void SetUp()
        {
            airlock = Substitute.For<IAirlock>();
            Trace.Configuration.Airlock = airlock;
            Trace.Configuration.InheritedFieldsWhitelist.Clear();
        }

        [Test]
        public void BeginSpan_should_inherit_custom_field_from_whitelist()
        {
            const string customKey = "customKey";
            const string customValue = "customValue";

            Trace.Configuration.InheritedFieldsWhitelist.Add(customKey);

            airlock.Push(
                Arg.Any<string>(),
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

            airlock.Received(2).Push(Arg.Any<string>(), Arg.Any<Span>());
        }
    }
}
