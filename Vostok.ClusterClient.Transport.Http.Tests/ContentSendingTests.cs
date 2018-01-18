using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Utilities;
using Vostok.Helpers;

namespace Vostok
{
    public class ContentSendingTests : TransportTestsBase
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(500)]
        [TestCase(4096)]
        [TestCase(1024 * 1024)]
        [TestCase(4 * 1024 * 1024)]
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