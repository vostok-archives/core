using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Commons.Utilities;
using Vostok.Helpers;

namespace Vostok
{
    public class ContentReceivingTests : TransportTestsBase
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(500)]
        [TestCase(4096)]
        [TestCase(1024*1024)]
        [TestCase(4*1024*1024)]
        public void Should_be_able_to_receive_content_of_given_size(int size)
        {
            var content = ThreadSafeRandom.NextBytes(size.Bytes());

            using (var server = TestServer.StartNew(
                ctx =>
                {
                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentLength64 = content.Length;
                    ctx.Response.OutputStream.Write(content, 0, content.Length);
                }))
            {
                var response = Send(Request.Put(server.Url));

                response.Content.ToArraySegment().Should().Equal(content);
            }
        }

        [Test]
        public void Should_read_response_body_greater_than_64k_with_non_successful_code()
        {
            var content = ThreadSafeRandom.NextBytes(100.Kilobytes());

            using (var server = TestServer.StartNew(
                ctx =>
                {
                    ctx.Response.StatusCode = 409;
                    ctx.Response.OutputStream.Write(content, 0, content.Length);
                }))
            {
                var response = Send(Request.Put(server.Url));

                response.Code.Should().Be(ResponseCode.Conflict);
                response.Content.ToArraySegment().Should().Equal(content);
            }
        }

        [Test]
        public void Should_read_response_body_without_content_length()
        {
            var content = ThreadSafeRandom.NextBytes(500.Kilobytes());

            using (var server = TestServer.StartNew(
                ctx =>
                {
                    ctx.Response.StatusCode = 200;
                    ctx.Response.OutputStream.Write(content, 0, content.Length);
                }))
            {
                var response = Send(Request.Put(server.Url));

                response.Content.ToArraySegment().Should().Equal(content);
            }
        }
    }
}