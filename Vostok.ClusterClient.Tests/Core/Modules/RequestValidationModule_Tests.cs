using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Logging;
using Xunit;

namespace Vostok.Clusterclient.Core.Modules
{
    public class RequestValidationModule_Tests
    {
        private readonly IRequestContext context;
        private readonly ILog log;

        private readonly RequestValidationModule module;

        public RequestValidationModule_Tests()
        {
            log = Substitute.For<ILog>();
            log
                .When(l => l.Log(Arg.Any<LogEvent>()))
                .Do(info => new ConsoleLog().Log(info.Arg<LogEvent>()));

            context = Substitute.For<IRequestContext>();
            context.Log.Returns(log);
            module = new RequestValidationModule();
        }

        [Fact]
        public void Should_return_incorrect_arguments_result_if_request_is_not_valid()
        {
            context.Request.Returns(CreateIncorrectRequest());

            module.ExecuteAsync(context, _ => null).Result.Status.Should().Be(ClusterResultStatus.IncorrectArguments);
        }

        [Fact]
        public void Should_log_an_error_message_when_request_is_not_valid()
        {
            context.Request.Returns(CreateIncorrectRequest());

            module.ExecuteAsync(context, _ => null).GetAwaiter().GetResult();

            log.Received(1).Log(Arg.Is<LogEvent>(evt => evt.Level == LogLevel.Error));
        }

        [Fact]
        public void Should_delegate_to_next_module_if_request_is_valid()
        {
            context.Request.Returns(CreateCorrectRequest());

            var task = Task.FromResult(ClusterResult.UnexpectedException(context.Request));

            module.ExecuteAsync(context, _ => task).Should().BeSameAs(task);
        }

        private static Request CreateCorrectRequest()
        {
            return Request.Get("foo/bar");
        }

        private static Request CreateIncorrectRequest()
        {
            return new Request("FUCK", new Uri("foo/bar", UriKind.Relative));
        }
    }
}
