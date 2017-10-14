using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class HeaderSendingTests : TransportTestsBase
    {
        public HeaderSendingTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory]
        [InlineData(HeaderNames.Accept, "text/html")]
        [InlineData(HeaderNames.AcceptCharset, "utf-8")]
        [InlineData(HeaderNames.AcceptEncoding, "*")]
        [InlineData(HeaderNames.Authorization, "Basic YWxhZGRpbjpvcGVuc2VzYW1l")]
        [InlineData(HeaderNames.ContentDisposition, "inline")]
        [InlineData(HeaderNames.ContentEncoding, "identity")]
        [InlineData(HeaderNames.ContentRange, "bytes 200-1000/67589")]
        [InlineData(HeaderNames.ContentType, "text/html; charset=utf-8")]
        [InlineData(HeaderNames.ETag, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [InlineData(HeaderNames.Host, "vm-service")]
        [InlineData(HeaderNames.IfMatch, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [InlineData(HeaderNames.IfNoneMatch, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [InlineData(HeaderNames.IfModifiedSince, "Wed, 21 Oct 2015 07:28:00 GMT")]
        [InlineData(HeaderNames.LastModified, "Wed, 21 Oct 2015 07:28:00 GMT")]
        [InlineData(HeaderNames.Location, "http://server:545/file")]
        [InlineData(HeaderNames.Range, "bytes=200-1000")]
        [InlineData(HeaderNames.Referer, "whatever")]
        [InlineData(HeaderNames.UserAgent, "Firefox")]
        [InlineData(HeaderNames.XKonturClientIdentity, "Abonents.Service")]
        [InlineData(HeaderNames.XKonturRequestPriority, "Sheddable")]
        [InlineData(HeaderNames.XKonturRequestTimeout, "345345345")]
        public void Should_correctly_transfer_given_header_to_server(string headerName, string headerValue)
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                var request = Request.Post(server.Url).WithHeader(headerName, headerValue);

                Send(request);

                server.LastRequest.Headers[headerName].Should().Be(headerValue);
            }
        }
    }
}