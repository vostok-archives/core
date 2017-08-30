using System;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Weighed.Leadership;
using Xunit;

namespace Vostok.Clusterclient.Core.Ordering.Weighed.Leadership
{
    public class AcceptedVerdictOr503LeaderDetector_Tests
    {
        private readonly Uri replica;
        private readonly AcceptedVerdictOr503LeaderDetector detector;

        public AcceptedVerdictOr503LeaderDetector_Tests()
        {
            replica = new Uri("http://replica");
            detector = new AcceptedVerdictOr503LeaderDetector();
        }

        [Fact]
        public void IsLeaderResult_should_return_true_for_result_with_accept_verdict()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.Accept, TimeSpan.Zero)).Should().BeTrue();
        }

        [Fact]
        public void IsLeaderResult_should_return_true_for_result_with_reject_verdict_but_503_response()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero)).Should().BeTrue();
        }

        [Fact]
        public void IsLeaderResult_should_return_false_for_result_with_reject_verdict()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.Reject, TimeSpan.Zero)).Should().BeFalse();
        }

        [Fact]
        public void IsLeaderResult_should_return_false_for_result_with_dontknow_verdict()
        {
            detector.IsLeaderResult(new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.DontKnow, TimeSpan.Zero)).Should().BeFalse();
        }
    }
}
