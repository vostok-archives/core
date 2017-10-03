using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class ContentSendingTests : TransportTestsBase
    {
        public ContentSendingTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(500)]
        [InlineData(4096)]
        [InlineData(1024 * 1024)]
        [InlineData(4 * 1024 * 1024)]
        public void Should_be_able_to_send_content_of_given_size(int size)
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                var content = ThreadSafeRandom.NextBytes(size.Bytes());

                var request = Request.Put(server.Url).WithContent(content);

                Send(request);

                server.LastRequest.Body.Should().Equal(content);
            }
        }

    }
}