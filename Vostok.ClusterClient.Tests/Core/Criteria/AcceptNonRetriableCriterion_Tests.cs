using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Criteria
{
    public class AcceptNonRetriableCriterion_Tests
    {
        private AcceptNonRetriableCriterion criterion;

        [SetUp]
        public void SetUp()
        {
            criterion = new AcceptNonRetriableCriterion();
        }

        [Test]
        public void Should_accept_an_error_response_with_dont_retry_header()
        {
            var response = new Response(ResponseCode.ServiceUnavailable, headers: Headers.Empty.Set(HeaderNames.XKonturDontRetry, ""));

            criterion.Decide(response).Should().Be(ResponseVerdict.Accept);
        }

        [Test]
        public void Should_know_nothing_about_an_error_response_without_dont_retry_header()
        {
            var response = new Response(ResponseCode.ServiceUnavailable, headers: Headers.Empty);

            criterion.Decide(response).Should().Be(ResponseVerdict.DontKnow);
        }
    }
}
