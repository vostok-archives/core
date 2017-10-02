using FluentAssertions;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport
{
    public class ConnectTimeoutHelper_Tests
    {
        private readonly ILog log;

        public ConnectTimeoutHelper_Tests(ITestOutputHelper outputHelper)
        {
            log = new TestOutputLog(outputHelper);
        }

        [Fact]
        public void Should_be_able_to_build_socket_check_delegate()
        {
            ConnectTimeoutHelper.CanCheckSocket.Should().BeTrue();
        }
    }
}