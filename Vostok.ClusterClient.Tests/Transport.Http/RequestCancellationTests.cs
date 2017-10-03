using System.Threading;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class RequestCancellationTests : TransportTestsBase
    {
        private readonly CancellationTokenSource tokenSource;
        private readonly CancellationToken token;

        public RequestCancellationTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        [Fact]
        public void Cancellation_should_work_correctly_on_already_canceled_token()
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                tokenSource.Cancel();

                SendWithCancellation(Request.Get(server.Url)).Code.Should().Be(ResponseCode.Canceled);
            }
        }

        [Fact]
        public void Cancellation_should_work_correctly_when_server_is_slow_to_respond()
        {
            using (var server = TestServer.StartNew(ctx =>
            {
                Thread.Sleep(1.Seconds());
                ctx.Response.StatusCode = 200;
            }))
            {
                SendWithCancellation(Request.Get(server.Url)).Code.Should().Be(ResponseCode.Canceled);
            }
        }

        [Fact]
        public void Cancellation_should_work_correctly_when_server_is_slow_to_transmit_response_body()
        {
            using (var server = TestServer.StartNew(ctx =>
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength64 = 10;

                for (var i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    ctx.Response.OutputStream.WriteByte(0xFF);
                }
            }))
            {
                SendWithCancellation(Request.Get(server.Url)).Code.Should().Be(ResponseCode.Canceled);
            }
        }

        [Fact]
        public void Cancellation_should_work_correctly_when_server_connection_cannot_be_established()
        {
            SendWithCancellation(Request.Get("http://193.54.62.128:6153/")).Code.Should().Be(ResponseCode.Canceled);
        }

        private Response SendWithCancellation(Request request)
        {
            var sendTask = transport.SendAsync(request, 1.Minutes(), token);

            tokenSource.CancelAfter(200.Milliseconds());

            return sendTask.GetAwaiter().GetResult();
        }

    }
}