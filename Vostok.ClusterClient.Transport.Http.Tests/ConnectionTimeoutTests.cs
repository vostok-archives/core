using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Helpers;

namespace Vostok
{
    public class ConnectionTimeoutTests : TransportTestsBase
    {
        private const string blackholeUrl = "http://193.42.113.67:5453/foo/bar";

        // TODO(iloktionov): Придумать что-то получше рандомного IP (например, hellion).
        [Test, Ignore("Not stable on Appveyor :(")]
        public void Should_timeout_on_connection_to_a_blackhole()
        {
            Transport.ConnectionTimeout = 250.Milliseconds();

            var task = Transport.SendAsync(Request.Get(blackholeUrl), 1.Minutes(), CancellationToken.None);

            task.Wait(2.Seconds()).Should().BeTrue();

            task.Result.Code.Should().Be(ResponseCode.ConnectFailure);
        }

        [Test]
        public void Should_not_timeout_on_connection_to_a_blackhole_if_connection_timeout_is_disabled()
        {
            Transport.ConnectionTimeout = null;

            var task = Transport.SendAsync(Request.Get(blackholeUrl), 1.Minutes(), CancellationToken.None);

            task.Wait(2.Seconds()).Should().BeFalse();
        }

        [Test]
        public void Should_not_timeout_when_server_is_just_slow()
        {
            Transport.ConnectionTimeout = 250.Milliseconds();

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