using System;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Weighed.Adaptive;
using Xunit;

namespace Vostok.Clusterclient.Core.Ordering.Weighed.Adaptive
{
    public class ResponseVerdictTuningPolicy_Tests
    {
        private readonly ResponseVerdictTuningPolicy policy;

        public ResponseVerdictTuningPolicy_Tests()
        {
            policy = new ResponseVerdictTuningPolicy();
        }

        [Theory]
        [InlineData(ResponseVerdict.Accept, AdaptiveHealthAction.Increase)]
        [InlineData(ResponseVerdict.Reject, AdaptiveHealthAction.Decrease)]
        [InlineData(ResponseVerdict.DontKnow, AdaptiveHealthAction.DontTouch)]
        public void Should_return_correct_action_for_given_response_verdict(ResponseVerdict verdict, AdaptiveHealthAction action)
        {
            var result = new ReplicaResult(new Uri("http://replica"), Responses.Timeout, verdict, TimeSpan.Zero);

            policy.SelectAction(result).Should().Be(action);
        }
    }
}
