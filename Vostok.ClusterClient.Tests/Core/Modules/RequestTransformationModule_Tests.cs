using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Strategies;
using Vostok.Clusterclient.Transforms;
using Vostok.Logging;
using Xunit;

namespace Vostok.Clusterclient.Core.Modules
{
    public class RequestTransformationModule_Tests
    {
        private readonly Request request1;
        private readonly Request request2;
        private readonly Request request3;
        private readonly RequestContext context;
        private RequestTransformationModule module;

        private readonly IRequestTransform transform1;
        private readonly IRequestTransform transform2;

        private readonly List<IRequestTransform> transforms;

        public RequestTransformationModule_Tests()
        {
            request1 = Request.Get("/1");
            request2 = Request.Get("/2");
            request3 = Request.Get("/3");

            context = new RequestContext(request1, Strategy.SingleReplica, Budget.Infinite, new ConsoleLog(), CancellationToken.None, null, int.MaxValue);

            transform1 = Substitute.For<IRequestTransform>();
            transform1.Transform(Arg.Any<Request>()).Returns(request2);

            transform2 = Substitute.For<IRequestTransform>();
            transform2.Transform(Arg.Any<Request>()).Returns(request3);

            transforms = new List<IRequestTransform> {transform1, transform2};

            module = new RequestTransformationModule(transforms);
        }

        [Fact]
        public void Should_not_modify_request_if_transforms_list_is_null()
        {
            module = new RequestTransformationModule(null);

            Execute();

            context.Request.Should().BeSameAs(request1);
        }

        [Fact]
        public void Should_not_modify_request_if_transforms_list_is_empty()
        {
            transforms.Clear();

            Execute();

            context.Request.Should().BeSameAs(request1);
        }

        [Fact]
        public void Should_apply_all_request_transforms_in_order()
        {
            Execute();

            context.Request.Should().BeSameAs(request3);

            Received.InOrder(
                () =>
                {
                    transform1.Transform(request1);
                    transform2.Transform(request2);
                });
        }

        private void Execute()
        {
            var taskSource = new TaskCompletionSource<ClusterResult>();

            var task = taskSource.Task;

            module.ExecuteAsync(context, _ => task).Should().BeSameAs(task);
        }
    }
}
