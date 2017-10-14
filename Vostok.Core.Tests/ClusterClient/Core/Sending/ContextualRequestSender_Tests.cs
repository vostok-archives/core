using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Sending;
using Vostok.Clusterclient.Strategies;
using NUnit.Framework;
using Vostok.Logging.Logs;

namespace Vostok.Clusterclient.Core.Sending
{
    public class ContextualRequestSender_Tests
    {
        private readonly Uri replica;
        private readonly Request request;
        private readonly ReplicaResult result;
        private readonly TimeSpan timeout;

        private readonly TaskCompletionSource<ReplicaResult> resultSource;
        private readonly IRequestSender baseSender;
        private readonly RequestContext context;
        private readonly ContextualRequestSender contextualSender;

        public ContextualRequestSender_Tests()
        {
            replica = new Uri("http://replica");
            request = Request.Get("foo/bar");
            result = new ReplicaResult(replica, new Response(ResponseCode.Ok), ResponseVerdict.Accept, 1.Milliseconds());
            timeout = 1.Minutes();

            resultSource = new TaskCompletionSource<ReplicaResult>();

            baseSender = Substitute.For<IRequestSender>();
            baseSender.SendToReplicaAsync(null, null, TimeSpan.Zero, CancellationToken.None).ReturnsForAnyArgs(_ => resultSource.Task);

            var log = new ConsoleLog();

            context = new RequestContext(request, Strategy.SingleReplica, Budget.WithRemaining(timeout), log, CancellationToken.None, null, int.MaxValue);
            contextualSender = new ContextualRequestSender(baseSender, context);
        }

        [Test]
        public void Should_add_default_replica_result_to_context_before_sending_request()
        {
            var sendTask = contextualSender.SendToReplicaAsync(replica, request, timeout, CancellationToken.None);

            var defaultResult = context.FreezeReplicaResults().Should().ContainSingle().Which;

            defaultResult.Replica.Should().BeSameAs(replica);
            defaultResult.Response.Should().BeSameAs(Responses.Unknown);
            defaultResult.Verdict.Should().Be(ResponseVerdict.DontKnow);

            CompleteSending();

            sendTask.GetAwaiter().GetResult();
        }

        [Test]
        public void Should_add_real_replica_result_to_context_after_sending_request()
        {
            var sendTask = contextualSender.SendToReplicaAsync(replica, request, timeout, CancellationToken.None);

            CompleteSending();

            sendTask.GetAwaiter().GetResult();

            context.FreezeReplicaResults().Should().ContainSingle().Which.Should().BeSameAs(result);
        }

        [Test]
        public void Should_return_result_from_base_request_sender()
        {
            var sendTask = contextualSender.SendToReplicaAsync(replica, request, timeout, CancellationToken.None);

            CompleteSending();

            sendTask.GetAwaiter().GetResult().Should().BeSameAs(result);
        }

        [Test]
        public void Should_pass_cancellation_token_to_base_request_sender()
        {
            var tokenSource = new CancellationTokenSource();

            CompleteSending();

            contextualSender.SendToReplicaAsync(replica, request, timeout, tokenSource.Token).GetAwaiter().GetResult();

            baseSender.Received().SendToReplicaAsync(replica, request, timeout, tokenSource.Token);
        }

        [Test]
        public void Should_pass_cancellation_exceptions_through()
        {
            Task.Run(() => resultSource.TrySetException(new OperationCanceledException()));

            Action action = () => contextualSender.SendToReplicaAsync(replica, request, timeout, CancellationToken.None).GetAwaiter().GetResult();

            action.ShouldThrow<OperationCanceledException>();
        }

        [Test]
        public void Should_set_canceled_result_for_replica_upon_seeing_cancellation_exception()
        {
            Task.Run(() => resultSource.TrySetException(new OperationCanceledException()));

            try
            {
                contextualSender.SendToReplicaAsync(replica, request, timeout, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }

            var replicaResult = context.FreezeReplicaResults().Should().ContainSingle().Which;

            replicaResult.Replica.Should().BeSameAs(replica);
            replicaResult.Response.Should().BeSameAs(Responses.Canceled);
            replicaResult.Verdict.Should().Be(ResponseVerdict.DontKnow);
        }

        private void CompleteSending()
        {
            Task.Run(() => resultSource.TrySetResult(result));
        }
    }
}
