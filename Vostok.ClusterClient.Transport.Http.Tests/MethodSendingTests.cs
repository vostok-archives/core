using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Helpers;

namespace Vostok
{
    public class MethodSendingTests : TransportTestsBase
    {
        [TestCaseSource(nameof(GetAllMethods))]
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