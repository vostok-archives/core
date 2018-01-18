using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Helpers;

namespace Vostok
{
    public class HeaderReceivingTests : TransportTestsBase
    {
        [TestCase(HeaderNames.ContentDisposition, "inline")]
        [TestCase(HeaderNames.ContentEncoding, "identity")]
        [TestCase(HeaderNames.ContentRange, "bytes 200-1000/67589")]
        [TestCase(HeaderNames.ContentType, "text/html; charset=utf-8")]
        [TestCase(HeaderNames.ETag, "\"bfc13a64729c4290ef5b2c2730249c88ca92d82d\"")]
        [TestCase(HeaderNames.Host, "vm-service")]
        [TestCase(HeaderNames.LastModified, "Wed, 21 Oct 2015 07:28:00 GMT")]
        [TestCase(HeaderNames.Location, "http://server:545/file")]
        [TestCase(HeaderNames.XKonturClientIdentity, "Abonents.Service")]
        [TestCase(HeaderNames.XKonturRequestPriority, "Sheddable")]
        [TestCase(HeaderNames.XKonturRequestTimeout, "345345345")]
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