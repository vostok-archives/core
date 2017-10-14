using System;
using System.Linq;
using FluentAssertions;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class RejectThrottlingErrorsCriterion_Tests
    {
        private RejectThrottlingErrorsCriterion criterion;

        [SetUp]
        public void SetUp()
        {
            criterion = new RejectThrottlingErrorsCriterion();
        }

        [Test]
        public void Should_reject_http_429_response_code()
        {
            criterion.Decide(new Response(ResponseCode.TooManyRequests)).Should().Be(ResponseVerdict.Reject);
        }

        [Test]
        public void Should_know_nothing_about_codes_which_are_not_throttling_errors()
        {
            var codes = Enum.GetValues(typeof (ResponseCode)).Cast<ResponseCode>().Where(code => code != ResponseCode.TooManyRequests);

            foreach (var code in codes)
            {
                criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.DontKnow);
            }
        }
    }
}
