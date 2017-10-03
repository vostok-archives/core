using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class HeaderReceivingTests : TransportTestsBase
    {
        public HeaderReceivingTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory]
        [InlineData(HeaderNames.ContentDisposition, "inline")]
        [InlineData(HeaderNames.ContentEncoding, "identity")]
        [InlineData(HeaderNames.ContentRange, "bytes 200-1000/67589")]
        [InlineData(HeaderNames.ContentType, "text/html; charset=utf-8")]
        [InlineData(HeaderNames.ETag, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [InlineData(HeaderNames.Host, "vm-service")]
        [InlineData(HeaderNames.LastModified, "Wed, 21 Oct 2015 07:28:00 GMT")]
        [InlineData(HeaderNames.Location, "http://server:545/file")]
        [InlineData(HeaderNames.XKonturClientIdentity, "Abonents.Service")]
        [InlineData(HeaderNames.XKonturRequestPriority, "Sheddable")]
        [InlineData(HeaderNames.XKonturRequestTimeout, "345345345")]
        public void Should_correctly_receive_given_header_from_server(string headerName, string headerValue)
        {
            using (var server = TestServer.StartNew(ctx =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.Headers.Set(headerName, headerValue);
            }))
            {
                var response = Send(Request.Post(server.Url));

                response.Headers[headerName].Should().Be(headerValue);
            }
        }
    }
}