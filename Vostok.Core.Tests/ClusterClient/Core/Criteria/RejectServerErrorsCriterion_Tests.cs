using System;
using System.Linq;
using FluentAssertions;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class RejectServerErrorsCriterion_Tests
    {
        private readonly RejectServerErrorsCriterion criterion;

        public RejectServerErrorsCriterion_Tests()
        {
            criterion = new RejectServerErrorsCriterion();
        }

        
        [TestCase(ResponseCode.InternalServerError)]
        [TestCase(ResponseCode.BadGateway)]
        [TestCase(ResponseCode.ServiceUnavailable)]
        [TestCase(ResponseCode.ProxyTimeout)]
        public void Should_reject_given_response_code(ResponseCode code)
        {
            criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.Reject);
        }

        [Test]
        public void Should_know_nothing_about_codes_which_are_not_server_errors()
        {
            var codes = Enum.GetValues(typeof (ResponseCode)).Cast<ResponseCode>().Where(code => !code.IsServerError());

            foreach (var code in codes)
            {
                criterion.Decide(new Response(code)).Should().Be(ResponseVerdict.DontKnow);
            }
        }

        [Test]
        public void Should_know_nothing_about_not_implemented_code()
        {
            criterion.Decide(new Response(ResponseCode.NotImplemented)).Should().Be(ResponseVerdict.DontKnow);
        }

        [Test]
        public void Should_know_nothing_about_http_version_not_supported_code()
        {
            criterion.Decide(new Response(ResponseCode.HttpVersionNotSupported)).Should().Be(ResponseVerdict.DontKnow);
        }
    }
}
