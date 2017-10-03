using System.Threading;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class ConnectionTimeoutTests : TransportTestsBase
    {
        private const string BlackholeUrl = "http://193.42.113.67:5453/foo/bar";

        public ConnectionTimeoutTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void Should_timeout_on_connection_to_a_blackhole()
        {
            transport.ConnectionTimeout = 250.Milliseconds();

            var task = transport.SendAsync(Request.Get(BlackholeUrl), 1.Minutes(), CancellationToken.None);

            task.Wait(2.Seconds()).Should().BeTrue();

            task.Result.Code.Should().Be(ResponseCode.ConnectFailure);
        }

        [Fact]
        public void Should_not_timeout_on_connection_to_a_blackhole_if_connection_timeout_is_disabled()
        {
            transport.ConnectionTimeout = null;

            var task = transport.SendAsync(Request.Get(BlackholeUrl), 1.Minutes(), CancellationToken.None);

            task.Wait(2.Seconds()).Should().BeFalse();
        }

        [Fact]
        public void Should_not_timeout_when_server_is_just_slow()
        {
            transport.ConnectionTimeout = 250.Milliseconds();

            using (var server = TestServer.StartNew(ctx =>
            {
                Thread.Sleep(2.Seconds());
                ctx.Response.StatusCode = 200;
            }))
            {
                Send(Request.Post(server.Url)).Code.Should().Be(ResponseCode.Ok);
            }
        }
    }
}