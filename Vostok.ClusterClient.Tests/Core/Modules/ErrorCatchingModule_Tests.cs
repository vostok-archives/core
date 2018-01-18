using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class ErrorCatchingModule_Tests
    {
        private IRequestContext context;
        private ILog log;
        private ErrorCatchingModule module;

        [SetUp]
        public void SetUp()
        {
            log = Substitute.For<ILog>();
            log
                .When(l => l.Log(Arg.Any<LogEvent>()))
                .Do(info => new ConsoleLog().Log(info.Arg<LogEvent>()));

            context = Substitute.For<IRequestContext>();
            context.Log.Returns(log);
            context.Request.Returns(Request.Get("foo/bar"));
            module = new ErrorCatchingModule();
        }

        [Test]
        public void Should_return_unexpected_exception_result_if_next_module_throws_an_error()
        {
            module.ExecuteAsync(context, _ => throw new Exception()).Result.Status.Should().Be(ClusterResultStatus.UnexpectedException);
        }

        [Test]
        public void Should_return_canceled_result_if_next_module_throws_a_cancellation_exception()
        {
            module.ExecuteAsync(context, _ => throw new OperationCanceledException()).Result.Status.Should().Be(ClusterResultStatus.Canceled);
        }

        [Test]
        public void Should_log_an_error_message_if_next_module_throws_an_error()
        {
            module.ExecuteAsync(context, _ => throw new Exception()).GetAwaiter().GetResult();

            log.Received(1).Log(Arg.Is<LogEvent>(evt => evt.Level == LogLevel.Error));
        }

        [Test]
        public void Should_not_log_an_error_message_if_next_module_throws_a_cancellation_error()
        {
            module.ExecuteAsync(context, _ => throw new OperationCanceledException()).GetAwaiter().GetResult();

            log.ReceivedCalls().Should().BeEmpty();
        }

        [Test]
        public void Should_delegate_to_next_module_if_no_exceptions_arise()
        {
            var task = Task.FromResult(ClusterResult.ReplicasNotFound(context.Request));

            module.ExecuteAsync(context, _ => task).Result.Should().BeSameAs(task.Result);
        }
    }
}
