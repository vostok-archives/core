using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Criteria
{
    public class ResponseClassifier_Tests
    {
        private readonly List<IResponseCriterion> criteria;
        private readonly ResponseClassifier classifier;
        private readonly Response response;

        public ResponseClassifier_Tests()
        {
            criteria = new List<IResponseCriterion>();
            classifier = new ResponseClassifier();
            response = Responses.Timeout;
        }

        [Test]
        public void Should_return_dont_know_verdict_for_empty_criteria_list()
        {
            classifier.Decide(response, criteria).Should().Be(ResponseVerdict.DontKnow);
        }

        [Test]
        public void Should_return_dont_know_verdict_if_no_criterion_knows_anything()
        {
            AddCriterion(ResponseVerdict.DontKnow);
            AddCriterion(ResponseVerdict.DontKnow);
            AddCriterion(ResponseVerdict.DontKnow);

            classifier.Decide(response, criteria).Should().Be(ResponseVerdict.DontKnow);
        }

        [Test]
        public void Should_stop_on_first_accepted_verdict()
        {
            AddCriterion(ResponseVerdict.DontKnow);
            AddCriterion(ResponseVerdict.DontKnow);
            AddCriterion(ResponseVerdict.Accept);
            AddCriterion(ResponseVerdict.Reject);

            classifier.Decide(response, criteria).Should().Be(ResponseVerdict.Accept);
        }

        [Test]
        public void Should_stop_on_first_rejected_verdict()
        {
            AddCriterion(ResponseVerdict.DontKnow);
            AddCriterion(ResponseVerdict.DontKnow);
            AddCriterion(ResponseVerdict.Reject);
            AddCriterion(ResponseVerdict.Accept);

            classifier.Decide(response, criteria).Should().Be(ResponseVerdict.Reject);
        }

        private void AddCriterion(ResponseVerdict verdict)
        {
            var criterion = Substitute.For<IResponseCriterion>();

            criterion.Decide(response).Returns(verdict);

            criteria.Add(criterion);
        }
    }
}
