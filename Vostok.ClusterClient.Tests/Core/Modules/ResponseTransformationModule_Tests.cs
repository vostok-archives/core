using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Transforms;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class ResponseTransformationModule_Tests
    {
        private IRequestContext context;
        private Response response1;
        private Response response2;
        private Response response3;
        private ClusterResult initialResult;
        private Func<IRequestContext, Task<ClusterResult>> nextModule;

        private IResponseTransform transform1;
        private IResponseTransform transform2;
        private List<IResponseTransform> transforms;
        private ResponseTransformationModule module;

        [SetUp]
        public void SetUp()
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

        [Test]
        public void Should_not_modify_response_if_transforms_list_is_null()
        {
            module = new ResponseTransformationModule(null);

            Execute().Should().BeSameAs(initialResult);
        }

        [Test]
        public void Should_not_modify_response_if_transforms_list_is_empty()
        {
            transforms.Clear();

            Execute().Should().BeSameAs(initialResult);
        }

        [Test]
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

        [Test]
        public void Should_preserve_result_status()
        {
            Execute().Status.Should().Be(initialResult.Status);
        }

        [Test]
        public void Should_preserve_replica_results()
        {
            Execute().ReplicaResults.Should().BeSameAs(initialResult.ReplicaResults);
        }

        [Test]
        public void Should_preserve_request_object_in_result()
        {
            Execute().Request.Should().BeSameAs(initialResult.Request);
        }

        [Test]
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
