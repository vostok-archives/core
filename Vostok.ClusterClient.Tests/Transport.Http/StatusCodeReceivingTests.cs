using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class StatusCodeReceivingTests : TransportTestsBase
    {
        public StatusCodeReceivingTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory]
        // 2xx
        [InlineData(ResponseCode.Ok)]
        [InlineData(ResponseCode.Created)]
        [InlineData(ResponseCode.Accepted)]
        [InlineData(ResponseCode.NoContent)]
        [InlineData(ResponseCode.ResetContent)]
        [InlineData(ResponseCode.PartialContent)]
        // 3xx
        [InlineData(ResponseCode.MultipleChoices)]
        [InlineData(ResponseCode.MovedPermanently)]
        [InlineData(ResponseCode.Found)]
        [InlineData(ResponseCode.SeeOther)]
        [InlineData(ResponseCode.NotModified)]
        [InlineData(ResponseCode.UseProxy)]
        [InlineData(ResponseCode.TemporaryRedirect)]
        // 4xx
        [InlineData(ResponseCode.BadRequest)]
        [InlineData(ResponseCode.Unauthorized)]
        [InlineData(ResponseCode.Forbidden)]
        [InlineData(ResponseCode.NotFound)]
        [InlineData(ResponseCode.MethodNotAllowed)]
        [InlineData(ResponseCode.RequestTimeout)]
        [InlineData(ResponseCode.Conflict)]
        [InlineData(ResponseCode.Gone)]
        [InlineData(ResponseCode.PreconditionFailed)]
        [InlineData(ResponseCode.TooManyRequests)]
        // 5xx
        [InlineData(ResponseCode.InternalServerError)]
        [InlineData(ResponseCode.NotImplemented)]
        [InlineData(ResponseCode.BadGateway)]
        [InlineData(ResponseCode.ServiceUnavailable)]
        public void Should_be_able_to_receive_given_response_code_from_server(ResponseCode code)
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = (int) code))
            {
                var response = Send(Request.Get(server.Url));

                response.Code.Should().Be((ResponseCode) code);
            }
        }
    }
}
