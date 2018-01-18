using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Helpers;

namespace Vostok
{
    public class StatusCodeReceivingTests : TransportTestsBase
    {
        // 2xx
        [TestCase(ResponseCode.Ok)]
        [TestCase(ResponseCode.Created)]
        [TestCase(ResponseCode.Accepted)]
        [TestCase(ResponseCode.NoContent)]
        [TestCase(ResponseCode.ResetContent)]
        [TestCase(ResponseCode.PartialContent)]
        // 3xx
        [TestCase(ResponseCode.MultipleChoices)]
        [TestCase(ResponseCode.MovedPermanently)]
        [TestCase(ResponseCode.Found)]
        [TestCase(ResponseCode.SeeOther)]
        [TestCase(ResponseCode.NotModified)]
        [TestCase(ResponseCode.UseProxy)]
        [TestCase(ResponseCode.TemporaryRedirect)]
        // 4xx
        [TestCase(ResponseCode.BadRequest)]
        [TestCase(ResponseCode.Unauthorized)]
        [TestCase(ResponseCode.Forbidden)]
        [TestCase(ResponseCode.NotFound)]
        [TestCase(ResponseCode.MethodNotAllowed)]
        [TestCase(ResponseCode.RequestTimeout)]
        [TestCase(ResponseCode.Conflict)]
        [TestCase(ResponseCode.Gone)]
        [TestCase(ResponseCode.PreconditionFailed)]
        [TestCase(ResponseCode.TooManyRequests)]
        // 5xx
        [TestCase(ResponseCode.InternalServerError)]
        [TestCase(ResponseCode.NotImplemented)]
        [TestCase(ResponseCode.BadGateway)]
        [TestCase(ResponseCode.ServiceUnavailable)]
        public void Should_be_able_to_receive_given_response_code_from_server(ResponseCode code)
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = (int) code))
            {
                var response = Send(Request.Get(server.Url));

                response.Code.Should().Be(code);
            }
        }
    }
}
