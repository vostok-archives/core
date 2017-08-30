using System;
using System.Linq;
using FluentAssertions;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class AlwaysRejectCriterion_Tests
    {
        [Fact]
        public void Should_reject_all_response_codes()
        {
            var criterion = new AlwaysRejectCriterion();

            var codes = Enum.GetValues(typeof (ResponseCode)).Cast<ResponseCode>();

            foreach (var code in codes)
            {
                criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.Reject);
            }
        }
    }
}
