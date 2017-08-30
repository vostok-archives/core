using System;
using System.Linq;
using FluentAssertions;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class RejectRedirectionsCriterion_Tests
    {
        private readonly RejectRedirectionsCriterion criterion;

        public RejectRedirectionsCriterion_Tests()
        {
            criterion = new RejectRedirectionsCriterion();
        }

        [Theory]
        [InlineData(ResponseCode.MultipleChoices)]
        [InlineData(ResponseCode.MovedPermanently)]
        [InlineData(ResponseCode.Found)]
        [InlineData(ResponseCode.SeeOther)]
        [InlineData(ResponseCode.UseProxy)]
        [InlineData(ResponseCode.TemporaryRedirect)]
        public void Should_reject_given_response_code(ResponseCode code)
        {
            criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.Reject);
        }

        [Fact]
        public void Should_know_nothing_about_codes_which_are_not_redirections()
        {
            var codes = Enum.GetValues(typeof (ResponseCode)).Cast<ResponseCode>().Where(code => !code.IsRedirection());

            foreach (var code in codes)
            {
                criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.DontKnow);
            }
        }

        [Fact]
        public void Should_know_nothing_about_not_modified_code()
        {
            criterion.Decide(new Response(ResponseCode.NotModified)).Should().Be(ResponseVerdict.DontKnow);
        }
    }
}
