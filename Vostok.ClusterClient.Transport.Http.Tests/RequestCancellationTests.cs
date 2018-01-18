using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Helpers;

namespace Vostok
{
    public class RequestCancellationTests : TransportTestsBase
    {
        private readonly CancellationTokenSource tokenSource;
        private readonly CancellationToken token;

        public RequestCancellationTests()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        [Test]
        public void Cancellation_should_work_correctly_on_already_canceled_token()
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                tokenSource.Cancel();

                SendWithCancellation(Request.Get(server.Url)).Code.Should().Be(ResponseCode.Canceled);
            }
        }

        [Test]
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

        [Test]
        public void Cancellation_should_work_correctly_when_server_connection_cannot_be_established()
        {
            SendWithCancellation(Request.Get("http://193.54.62.128:6153/")).Code.Should().Be(ResponseCode.Canceled);
        }

        private Response SendWithCancellation(Request request)
        {
            var sendTask = Transport.SendAsync(request, 1.Minutes(), token);

            tokenSource.CancelAfter(200.Milliseconds());

            return sendTask.GetAwaiter().GetResult();
        }

    }
}