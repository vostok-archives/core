using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Airlock;
using Vostok.Configuration;

namespace Vostok.Tracing
{
    public class Trace_Tests
    {
        private IAirlockClient airlockClient;

        [SetUp]
        public void SetUp()
        {
            airlockClient = Substitute.For<IAirlockClient>();
            VostokConfiguration.Project = () => "proj";
            VostokConfiguration.Environment = () => "env";
            VostokConfiguration.Service = () => "serv";
            VostokConfiguration.AirlockClient = airlockClient;
            VostokConfiguration.Tracing.InheritedFieldsWhitelist.Clear();
        }

        [Test]
        public void BeginSpan_should_inherit_custom_field_from_whitelist()
        {
            const string customKey = "customKey";
            const string customValue = "customValue";

            VostokConfiguration.Tracing.InheritedFieldsWhitelist.Add(customKey);

            airlockClient.Push(
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

            airlockClient.Received(2).Push(Arg.Any<string>(), Arg.Any<Span>());
        }
    }
}
