using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Weighed.Adaptive;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Weighed.Adaptive
{
    public class CompositeTuningPolicy_Tests
    {
        
        [TestCase(AdaptiveHealthAction.DontTouch)]
        [TestCase(AdaptiveHealthAction.DontTouch, AdaptiveHealthAction.DontTouch, AdaptiveHealthAction.DontTouch)]
        [TestCase(AdaptiveHealthAction.Increase, AdaptiveHealthAction.DontTouch, AdaptiveHealthAction.Increase)]
        [TestCase(AdaptiveHealthAction.Increase, AdaptiveHealthAction.Increase, AdaptiveHealthAction.DontTouch)]
        [TestCase(AdaptiveHealthAction.Decrease, AdaptiveHealthAction.DontTouch, AdaptiveHealthAction.Decrease)]
        [TestCase(AdaptiveHealthAction.Decrease, AdaptiveHealthAction.Decrease, AdaptiveHealthAction.DontTouch)]
        [TestCase(AdaptiveHealthAction.Decrease, AdaptiveHealthAction.Decrease, AdaptiveHealthAction.Increase)]
        [TestCase(AdaptiveHealthAction.Decrease, AdaptiveHealthAction.Increase, AdaptiveHealthAction.Decrease)]
        public void Should_return_correct_action_based_on_constituents_decisions(AdaptiveHealthAction expected, params AdaptiveHealthAction[] actions)
        {
            var result = new ReplicaResult(new Uri("http://replica"), Responses.Timeout, ResponseVerdict.Accept, TimeSpan.Zero);

            var policies = new IAdaptiveHealthTuningPolicy[actions.Length];

            for (var i = 0; i < actions.Length; i++)
            {
                policies[i] = Substitute.For<IAdaptiveHealthTuningPolicy>();
                policies[i].SelectAction(result).Returns(actions[i]);
            }

            var compositePolicy = new CompositeTuningPolicy(policies);

            compositePolicy.SelectAction(result).Should().Be(expected);
        }
    }
}
