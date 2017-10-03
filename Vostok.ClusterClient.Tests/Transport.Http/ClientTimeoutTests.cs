using System.Threading;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class ClientTimeoutTests : TransportTestsBase
    {
        public ClientTimeoutTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(0.1)]
        [InlineData(0.4)]
        public void Should_return_timeout_response_without_actually_sending_when_request_timeout_is_less_or_equal_to_one_millisecond(int timeout)
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                var response = Send(Request.Get(server.Url), timeout.Milliseconds());

                response.Code.Should().Be(ResponseCode.RequestTimeout);

                server.LastRequest.Should().BeNull();
            }
        }

        [Fact]
        public void Should_return_server_response_if_it_comes_in_time()
        {
            using (var server = TestServer.StartNew(ctx =>
            {
                Thread.Sleep(1.Seconds());
                ctx.Response.StatusCode = 200;
            }))
            {
                var response = Send(Request.Get(server.Url), 3.Seconds());

                response.Code.Should().Be(ResponseCode.Ok);
            }
        }

        [Fact]
        public void Should_return_timeout_response_when_server_responds_too_late()
        {
            using (var server = TestServer.StartNew(
                ctx =>
                {
                    Thread.Sleep(1.Seconds());
                    ctx.Response.StatusCode = 200;
                }))
            {
                var response = Send(Request.Get(server.Url), 200.Milliseconds());

                response.Code.Should().Be(ResponseCode.RequestTimeout);
            }
        }
    }
}