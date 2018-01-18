using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Criteria
{
    public class RejectNetworkErrorsCriterion_Tests
    {
        private RejectNetworkErrorsCriterion criterion;

        [SetUp]
        public void SetUp()
        {
            criterion = new RejectNetworkErrorsCriterion();
        }

        
        [TestCase(ResponseCode.RequestTimeout)]
        [TestCase(ResponseCode.ConnectFailure)]
        [TestCase(ResponseCode.SendFailure)]
        [TestCase(ResponseCode.ReceiveFailure)]
        public void Should_reject_given_response_code(ResponseCode code)
        {
            criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.Reject);
        }

        [Test]
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
