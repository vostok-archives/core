using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Transforms;
using Xunit;

namespace Vostok.Clusterclient.Core.Modules
{
    public class ResponseTransformationModule_Tests
    {
        private readonly IRequestContext context;
        private readonly Response response1;
        private readonly Response response2;
        private readonly Response response3;
        private readonly ClusterResult initialResult;
        private readonly Func<IRequestContext, Task<ClusterResult>> nextModule;

        private readonly IResponseTransform transform1;
        private readonly IResponseTransform transform2;
        private readonly List<IResponseTransform> transforms;
        private ResponseTransformationModule module;

        public ResponseTransformationModule_Tests()
        {
            context = Substitute.For<IRequestContext>();
            context.Request.Returns(Request.Get("foo/bar"));

            response1 = new Response(ResponseCode.Ok);
            response2 = new Response(ResponseCode.Ok);
            response3 = new Response(ResponseCode.Ok);

            initialResult = new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), response1, context.Request);
            nextModule = _ => Task.FromResult(initialResult);

            transform1 = Substitute.For<IResponseTransform>();
            transform1.Transform(Arg.Any<Response>()).Returns(response2);

            transform2 = Substitute.For<IResponseTransform>();
            transform2.Transform(Arg.Any<Response>()).Returns(response3);

            transforms = new List<IResponseTransform> {transform1, transform2};

            module = new ResponseTransformationModule(transforms);
        }

        [Fact]
        public void Should_not_modify_response_if_transforms_list_is_null()
        {
            module = new ResponseTransformationModule(null);

            Execute().Should().BeSameAs(initialResult);
        }

        [Fact]
        public void Should_not_modify_response_if_transforms_list_is_empty()
        {
            transforms.Clear();

            Execute().Should().BeSameAs(initialResult);
        }

        [Fact]
        public void Should_apply_all_response_transforms_in_order()
        {
            Execute().Response.Should().BeSameAs(response3);

            Received.InOrder(
                () =>
                {
                    transform1.Transform(response1);
                    transform2.Transform(response2);
                });
        }

        [Fact]
        public void Should_preserve_result_status()
        {
            Execute().Status.Should().Be(initialResult.Status);
        }

        [Fact]
        public void Should_preserve_replica_results()
        {
            Execute().ReplicaResults.Should().BeSameAs(initialResult.ReplicaResults);
        }

        [Fact]
        public void Should_preserve_request_object_in_result()
        {
            Execute().Request.Should().BeSameAs(initialResult.Request);
        }

        [Fact]
        public void Should_preserve_whole_result_object_if_response_does_not_get_changed()
        {
            transform2.Transform(Arg.Any<Response>()).Returns(initialResult.Response);

            Execute().Should().BeSameAs(initialResult);
        }

        private ClusterResult Execute()
        {
            return module.ExecuteAsync(context, nextModule).GetAwaiter().GetResult();
        }
    }
}
