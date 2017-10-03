using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Vostok.Airlock;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Strategies;
using Vostok.Tracing;
using Xunit;

namespace Vostok.Clusterclient.Core.Modules
{
    public class TracingModule_Test
    {
        private readonly TracingModule tracingModule;
        private readonly IAirlock airlock;

        public TracingModule_Test()
        {
            airlock = Substitute.For<IAirlock>();
            Trace.Configuration.Airlock = airlock;
            tracingModule = new TracingModule("serviceName");
        }

        [Fact]
        public async Task ExecuteAsync_should_create_trace()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var request = new Request("GET", new Uri("vostok/process?p1=p", UriKind.Relative));
            requestContext.Request.Returns(request);
            requestContext.Strategy.Returns(new ParallelRequestStrategy(2));
            requestContext.OperationName.Returns((string) null);
            var response = new Response(ResponseCode.Conflict);
            var clusterResult = new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), response, request);
            var expectedAnnotations = new Dictionary<string, string>
            {
                ["kind"] = "cluster-client",
                ["component"] = "cluster-client",
                ["cluster.strategy"] = "ParallelRequestStrategy",
                ["cluster.status"] = "Success",
                ["http.url"] = "vostok/process",
                ["http.method"] = "GET",
                ["http.responseСontentLength"] = "0",
                ["http.code"] = "409",
                ["serviceName"] = "serviceName"
            };
            airlock.Push(Arg.Any<string>(), Arg.Do<Span>(span =>
            {
                span.OperationName.Should().Be("vostok/process");
                span.Annotations.ShouldBeEquivalentTo(expectedAnnotations);
            }));

            await tracingModule.ExecuteAsync(requestContext, x => Task.FromResult(clusterResult)).ConfigureAwait(false);

            airlock.Received().Push(Arg.Any<string>(), Arg.Any<Span>());
        }
    }
}