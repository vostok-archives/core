using System.Collections.Generic;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Retry;
using Xunit;

namespace Vostok.Clusterclient.Core.Retry
{
    public class NeverRetryPolicy_Tests
    {
        private readonly NeverRetryPolicy policy;

        public NeverRetryPolicy_Tests()
        {
            policy = new NeverRetryPolicy();
        }

        [Fact]
        public void NeedToRetry_should_always_return_false()
        {
            policy.NeedToRetry(new List<ReplicaResult>()).Should().BeFalse();
        }
    }
}
