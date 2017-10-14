using System;
using System.Linq;
using FluentAssertions;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class RejectNetworkErrorsCriterion_Tests
    {
        private readonly RejectNetworkErrorsCriterion criterion;

        public RejectNetworkErrorsCriterion_Tests()
        {
            criterion = new RejectNetworkErrorsCriterion();
        }

        [Theory]
        [InlineData(ResponseCode.RequestTimeout)]
        [InlineData(ResponseCode.ConnectFailure)]
        [InlineData(ResponseCode.SendFailure)]
        [InlineData(ResponseCode.ReceiveFailure)]
        public void Should_reject_given_response_code(ResponseCode code)
        {
            criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.Reject);
        }

        [Fact]
        public void Should_know_nothing_about_codes_which_are_not_network_errors()
        {
            var codes = Enum.GetValues(typeof (ResponseCode)).Cast<ResponseCode>().Where(code => !code.IsNetworkError());

            foreach (var code in codes)
            {
                criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.DontKnow);
            }
        }
    }
}
