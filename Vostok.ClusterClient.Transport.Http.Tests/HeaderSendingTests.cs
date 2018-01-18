using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Helpers;

namespace Vostok
{
    public class HeaderSendingTests : TransportTestsBase
    {
        [TestCase(HeaderNames.Accept, "text/html")]
        [TestCase(HeaderNames.AcceptCharset, "utf-8")]
        [TestCase(HeaderNames.AcceptEncoding, "*")]
        [TestCase(HeaderNames.Authorization, "Basic YWxhZGRpbjpvcGVuc2VzYW1l")]
        [TestCase(HeaderNames.ContentDisposition, "inline")]
        [TestCase(HeaderNames.ContentEncoding, "identity")]
        [TestCase(HeaderNames.ContentRange, "bytes 200-1000/67589")]
        [TestCase(HeaderNames.ContentType, "text/html; charset=utf-8")]
        [TestCase(HeaderNames.ETag, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [TestCase(HeaderNames.Host, "vm-service")]
        [TestCase(HeaderNames.IfMatch, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [TestCase(HeaderNames.IfNoneMatch, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [TestCase(HeaderNames.IfModifiedSince, "Wed, 21 Oct 2015 07:28:00 GMT")]
        [TestCase(HeaderNames.LastModified, "Wed, 21 Oct 2015 07:28:00 GMT")]
        [TestCase(HeaderNames.Location, "http://server:545/file")]
        [TestCase(HeaderNames.Range, "bytes=200-1000")]
        [TestCase(HeaderNames.Referer, "whatever")]
        [TestCase(HeaderNames.UserAgent, "Firefox")]
        [TestCase(HeaderNames.XKonturClientIdentity, "Abonents.Service")]
        [TestCase(HeaderNames.XKonturRequestPriority, "Sheddable")]
        [TestCase(HeaderNames.XKonturRequestTimeout, "345345345")]
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