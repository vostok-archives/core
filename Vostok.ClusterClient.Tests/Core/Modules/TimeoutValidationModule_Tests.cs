using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.ClusterClient.Tests.Helpers;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Modules
{
    public class TimeoutValidationModule_Tests
    {
        private IRequestContext context;
        private ILog log;
        private TimeoutValidationModule module;

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
            module = new TimeoutValidationModule();
        }

        [Test]
        public void Should_return_incorrect_arguments_result_if_timeout_is_negative()
        {
            var budget = Budget.WithRemaining(-1.Seconds());

            context.Budget.Returns(budget);

            module.ExecuteAsync(context, _ => null).Result.Status.Should().Be(ClusterResultStatus.IncorrectArguments);
        }

        [Test]
        public void Should_log_an_error_message_if_timeout_is_negative()
        {
            var budget = Budget.WithRemaining(-1.Seconds());

            context.Budget.Returns(budget);

            module.ExecuteAsync(context, _ => null).GetAwaiter().GetResult();

            log.Received(1).Log(Arg.Is<LogEvent>(evt => evt.Level == LogLevel.Error));
        }

        [Test]
        public void Should_return_time_expired_result_if_time_budget_is_already_expired()
        {
            var budget = Budget.WithRemaining(0.Seconds());

            context.Budget.Returns(budget);

            module.ExecuteAsync(context, _ => null).Result.Status.Should().Be(ClusterResultStatus.TimeExpired);
        }

        [Test]
        public void Should_log_an_error_message_if_time_budget_is_already_expired()
        {
            var budget = Budget.WithRemaining(0.Seconds());

            context.Budget.Returns(budget);

            module.ExecuteAsync(context, _ => null).GetAwaiter().GetResult();

            log.Received(1).Log(Arg.Is<LogEvent>(evt => evt.Level == LogLevel.Error));
        }

        [Test]
        public void Should_delegate_to_next_module_if_timeout_is_valid()
        {
            var budget = Budget.WithRemaining(5.Seconds());

            context.Budget.Returns(budget);

            var task = Task.FromResult(ClusterResult.UnexpectedException(context.Request));

            module.ExecuteAsync(context, _ => task).Should().BeSameAs(task);
        }
    }
}
