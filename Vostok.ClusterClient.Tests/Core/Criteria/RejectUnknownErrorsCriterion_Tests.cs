using System;
using System.Linq;
using FluentAssertions;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class RejectUnknownErrorsCriterion_Tests
    {
        private readonly RejectUnknownErrorsCriterion criterion;

        public RejectUnknownErrorsCriterion_Tests()
        {
            criterion = new RejectUnknownErrorsCriterion();
        }

        [Theory]
        [InlineData(ResponseCode.Unknown)]
        [InlineData(ResponseCode.UnknownFailure)]
        public void Should_reject_given_response_code(ResponseCode code)
        {
            criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.Reject);
        }

        [Fact]
        public void Should_know_nothing_about_codes_which_are_not_unknown_errors()
        {
            var codes = Enum.GetValues(typeof (ResponseCode)).Cast<ResponseCode>().Where(code => code != ResponseCode.Unknown && code != ResponseCode.UnknownFailure);

            foreach (var code in codes)
            {
                criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.DontKnow);
            }
        }
    }
}
