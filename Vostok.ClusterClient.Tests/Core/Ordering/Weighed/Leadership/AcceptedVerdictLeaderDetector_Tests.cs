using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Weighed.Leadership;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Weighed.Leadership
{
    public class AcceptedVerdictLeaderDetector_Tests
    {
        private Uri replica;
        private AcceptedVerdictLeaderDetector detector;

        [SetUp]
        public void SetUp()
        {
            replica = new Uri("http://replica");
            detector = new AcceptedVerdictLeaderDetector();
        }

        [Test]
        public void IsLeaderResult_should_return_true_for_result_with_accept_verdict()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.Accept, TimeSpan.Zero)).Should().BeTrue();
        }

        [Test]
        public void IsLeaderResult_should_return_false_for_result_with_reject_verdict()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero)).Should().BeFalse();
        }

        [Test]
        public void IsLeaderResult_should_return_false_for_result_with_dontknow_verdict()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.DontKnow, TimeSpan.Zero)).Should().BeFalse();
        }
    }
}
