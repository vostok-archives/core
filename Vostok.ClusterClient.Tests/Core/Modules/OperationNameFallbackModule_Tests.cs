using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Flow;
using Vostok.Tracing;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    [TestFixture]
    internal class OperationNameFallbackModule_Tests
    {
        private Request request;
        private IRequestContext context;
        private OperationNameFallbackModule module;

        [SetUp]
        public void TestSetup()
        {
            Context.Properties.RemoveProperty(TracingAnnotationNames.Operation);

            request = Request.Get("foo/bar/6D48F4FC-49EF-444E-A974-24E5F00DAF1E/?a=b");

            context = Substitute.For<IRequestContext>();
            context.Request.Returns(request);

            module = new OperationNameFallbackModule();
        }

        [TearDown]
        public void TearDown()
        {
            Context.Properties.RemoveProperty(TracingAnnotationNames.Operation);
        }

        [Test]
        public void Should_set_operation_name_in_context_when_none_was_provided_earlier()
        {
            Execute(() => Context.Properties.Current[TracingAnnotationNames.Operation].Should().Be("GET foo/bar/{guid}"));
        }

        [Test]
        public void Should_remove_fallback_operation_name_from_context_as_soon_as_request_completes()
        {
            Execute(() => {});

            Context.Properties.Current.ContainsKey(TracingAnnotationNames.Operation).Should().BeFalse();
        }

        [Test]
        public void Should_not_override_existing_operation_name_in_context()
        {
            Context.Properties.SetProperty(TracingAnnotationNames.Operation, "bullshit");

            Execute(() => Context.Properties.Current[TracingAnnotationNames.Operation].Should().Be("bullshit"));
        }

        private void Execute(Action payload)
        {
            var result = new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), new Response(ResponseCode.Ok), request);

            var resultTask = Task.FromResult(result);

            var moduleTask = module.ExecuteAsync(
                context,
                _ =>
                {
                    payload();
                    return resultTask;
                });

            moduleTask.GetAwaiter().GetResult().Should().BeSameAs(result);
        }
    }
}
