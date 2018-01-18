using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Sending;
using Vostok.Clusterclient.Strategies;
using Vostok.ClusterClient.Tests.Helpers;

#pragma warning disable 4014

namespace Vostok.ClusterClient.Tests.Core.Strategies
{
    public class ParallelRequestStrategy_Tests
    {
        private Uri[] replicas;
        private Request request;
        private IRequestSender sender;
        private Dictionary<Uri, TaskCompletionSource<ReplicaResult>> resultSources;
        private CancellationToken token;

        private ParallelRequestStrategy strategy;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("foo/bar");
            replicas = Enumerable.Range(0, 10).Select(i => new Uri($"http://replica-{i}/")).ToArray();
            resultSources = replicas.ToDictionary(r => r, _ => new TaskCompletionSource<ReplicaResult>());

            sender = Substitute.For<IRequestSender>();
            sender
                .SendToReplicaAsync(Arg.Any<Uri>(), Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
                .Returns(info => resultSources[info.Arg<Uri>()].Task);

            token = new CancellationTokenSource().Token;

            strategy = new ParallelRequestStrategy(3);
        }

        [Test]
        public void Ctor_should_throw_when_given_negative_parallelism_level()
        {
            Action action = () => new ParallelRequestStrategy(-1);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_when_given_zero_parallelism_level()
        {
            Action action = () => new ParallelRequestStrategy(0);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_immediately_fire_several_requests_to_reach_parallelism_level()
        {
            DropSynchronizationContext();

            strategy.SendAsync(request, sender, Budget.Infinite, replicas, replicas.Length, token);

            sender.ReceivedCalls().Should().HaveCount(3);
        }

        [Test]
        public void Should_fire_initial_requests_with_whole_remaining_time_budget()
        {
            DropSynchronizationContext();

            strategy.SendAsync(request, sender, Budget.WithRemaining(5.Seconds()), replicas, replicas.Length, token);

            sender.Received(1).SendToReplicaAsync(replicas[0], request, 5.Seconds(), Arg.Any<CancellationToken>());
            sender.Received(1).SendToReplicaAsync(replicas[1], request, 5.Seconds(), Arg.Any<CancellationToken>());
            sender.Received(1).SendToReplicaAsync(replicas[2], request, 5.Seconds(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_fail_with_bugcheck_exception_if_replicas_enumerable_is_insufficient()
        {
            DropSynchronizationContext();

            var task = strategy.SendAsync(request, sender, Budget.WithRemaining(5.Seconds()), replicas.Take(2).ToArray(), replicas.Length, token);

            task.IsFaulted.Should().BeTrue();
            Debug.Assert(task.Exception != null, "task.Exception != null");
            task.Exception.InnerExceptions.Single().Should().BeOfType<InvalidOperationException>().Which.ShouldBePrinted();
        }

        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Should_stop_when_any_of_the_requests_ends_with_accepted_response(int replicaIndex)
        {
            DropSynchronizationContext();

            var task = strategy.SendAsync(request, sender, Budget.Infinite, replicas, replicas.Length, token);

            CompleteRequest(replicas[replicaIndex], ResponseVerdict.Accept);

            task.IsCompleted.Should().BeTrue();
        }

        [Test]
        public void Should_issue_another_request_when_a_pending_one_ends_with_rejected_status()
        {
            DropSynchronizationContext();

            var task = strategy.SendAsync(request, sender, Budget.Infinite, replicas, replicas.Length, token);

            CompleteRequest(replicas[1], ResponseVerdict.Reject);

            task.IsCompleted.Should().BeFalse();

            sender.Received(1).SendToReplicaAsync(replicas[3], request, Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Should_stop_when_all_replicas_ended_up_returning_rejected_statuses()
        {
            DropSynchronizationContext();

            var task = strategy.SendAsync(request, sender, Budget.Infinite, replicas, replicas.Length, token);

            foreach (var replica in replicas)
            {
                CompleteRequest(replica, ResponseVerdict.Reject);
            }

            task.IsCompleted.Should().BeTrue();

            sender.ReceivedCalls().Should().HaveCount(replicas.Length);
        }

        [Test]
        public void Should_fire_initial_requests_to_all_replicas_if_parallelism_level_is_greater_than_replicas_count()
        {
            DropSynchronizationContext();

            strategy = new ParallelRequestStrategy(int.MaxValue);

            strategy.SendAsync(request, sender, Budget.WithRemaining(5.Seconds()), replicas, replicas.Length, token);

            sender.ReceivedCalls().Should().HaveCount(replicas.Length);

            foreach (var replica in replicas)
            {
                sender.Received(1).SendToReplicaAsync(replica, request, Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
            }
        }

        [Test]
        public void Should_cancel_remaining_requests_when_receiving_accepted_result()
        {
            DropSynchronizationContext();

            var tokens = new List<CancellationToken>();

            sender
                .When(s => s.SendToReplicaAsync(Arg.Any<Uri>(), Arg.Any<Request>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>()))
                .Do(info => tokens.Add(info.Arg<CancellationToken>()));

            strategy = new ParallelRequestStrategy(int.MaxValue);

            var sendTask = strategy.SendAsync(request, sender, Budget.WithRemaining(5.Seconds()), replicas, replicas.Length, token);

            CompleteRequest(replicas.Last(), ResponseVerdict.Accept);

            sendTask.GetAwaiter().GetResult();

            tokens.Should().HaveCount(replicas.Length);

            foreach (var t in tokens)
            {
                t.IsCancellationRequested.Should().BeTrue();
            }
        }

        private void CompleteRequest(Uri replica, ResponseVerdict verdict)
        {
            resultSources[replica].TrySetResult(new ReplicaResult(replica, new Response(ResponseCode.Ok), verdict, TimeSpan.Zero));
        }

        private static void DropSynchronizationContext()
        {
            SynchronizationContext.SetSynchronizationContext(null);
        }
    }
}
