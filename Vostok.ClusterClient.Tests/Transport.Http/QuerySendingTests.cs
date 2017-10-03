using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class QuerySendingTests : TransportTestsBase
    {
        public QuerySendingTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void Should_correctly_transfer_query_parameters_to_server()
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                var request = Request
                    .Get(server.Url)
                    .WithAdditionalQueryParameter("key1", "value1")
                    .WithAdditionalQueryParameter("key2", "value2");

                Send(request);

                server.LastRequest.Query["key1"].Should().Be("value1");
                server.LastRequest.Query["key2"].Should().Be("value2");
            }
        }
    }
}
