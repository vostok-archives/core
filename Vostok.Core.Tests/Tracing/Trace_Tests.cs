using System.Collections.Generic;
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
        }

        [Test]
        public void BeginSpan_should_inherit_custom_field()
        {
            const string customKey = "customKey";
            const string customValue = "customValue";
            Trace.Configuration.InheritableProperties.Add(customKey);
            airlock.Push(Arg.Any<string>(), Arg.Do<Span>(x =>
            {
                x.Annotations.Keys.Should().Contain(customKey);
                x.Annotations[customKey].Should().Contain(customValue);
            }));

            using (var span = Trace.BeginSpan(null))
            {
                span.SetAnnotation(customKey, customValue);
                using (Trace.BeginSpan(null))
                {
                    
                }
            }

            airlock.Received(2).Push(Arg.Any<string>(), Arg.Any<Span>());
        }

        [Test]
        public void BeginSpan_should_inherit_operation_name_when_operationName_is_null()
        {
            var writeOperaionNames = new List<string>();
            airlock.Push(Arg.Any<string>(), Arg.Do<Span>(x => writeOperaionNames.Add(x.OperationName)));

            using (Trace.BeginSpan("operationName"))
            {
                using (Trace.BeginSpan(null))
                {
                    
                }
            }

            writeOperaionNames.ShouldBeEquivalentTo(new[] { "operationName", "operationName" });
        }

        [Test]
        public void BeginSpan_should_not_inherit_operation_name_when_operationName_is_not_null()
        {
            var writeOperaionNames = new List<string>();
            airlock.Push(Arg.Any<string>(), Arg.Do<Span>(x => writeOperaionNames.Add(x.OperationName)));

            using (Trace.BeginSpan("operationName"))
            {
                using (Trace.BeginSpan("operationName2"))
                {
                    
                }
            }

            writeOperaionNames.ShouldBeEquivalentTo(new[] {"operationName", "operationName2"});
        }

        [Test]
        public void BeginSpan_should_inherit_operation_name_from_parent()
        {
            var writeOperaionNames = new List<string>();
            airlock.Push(Arg.Any<string>(), Arg.Do<Span>(x => writeOperaionNames.Add(x.OperationName)));

            using (Trace.BeginSpan("operationName"))
            {
                using (Trace.BeginSpan("operationName2"))
                {
                    
                }

                using (Trace.BeginSpan(null))
                {
                    
                }
            }

            writeOperaionNames.ShouldBeEquivalentTo(new[] {"operationName", "operationName2", "operationName" });
        }
    }
}