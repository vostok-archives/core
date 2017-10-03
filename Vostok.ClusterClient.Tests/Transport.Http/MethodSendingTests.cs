using System.Collections.Generic;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transport.Http.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Vostok.Clusterclient.Transport.Http
{
    public class MethodSendingTests : TransportTestsBase
    {
        public MethodSendingTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Theory]
        [MemberData(nameof(GetAllMethods))]
        public void Should_be_able_to_send_requests_with_given_method(string method)
        {
            using (var server = TestServer.StartNew(ctx => ctx.Response.StatusCode = 200))
            {
                Send(new Request(method, server.Url));

                server.LastRequest.Method.Should().Be(method);
            }
        }

        public static IEnumerable<object[]> GetAllMethods()
        {
            foreach (var method in RequestMethods.All)
            {
                yield return new object[] {method};
            }
        }
    }
}