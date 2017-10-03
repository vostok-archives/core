using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class ConnectionFailureTests : TransportTestsBase
    {
        public ConnectionFailureTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void Should_return_ConnectFailure_code_when_request_url_is_wrong()
        {
            var response = Send(Request.Get("http://255.255.255.255/"));

            response.Code.Should().Be(ResponseCode.ConnectFailure);
        }

        [Fact]
        public void Should_return_ConnectFailure_code_when_request_url_contains_unresolvable_hostname()
        {
            var response = Send(Request.Get("http://sdoghisguhodfgkdjfgdsfgj:7545/"));

            response.Code.Should().Be(ResponseCode.ConnectFailure);
        }

        [Fact]
        public void Should_return_ConnectFailure_code_when_server_does_not_listen_on_needed_port()
        {
            var response = Send(Request.Get($"http://localhost:{FreeTcpPortFinder.GetFreePort()}/"));

            response.Code.Should().Be(ResponseCode.ConnectFailure);
        }
    }
}