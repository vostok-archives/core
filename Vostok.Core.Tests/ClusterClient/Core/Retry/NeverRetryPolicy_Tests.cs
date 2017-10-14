using System.Collections.Generic;
using FluentAssertions;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Retry;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Retry
{
    public class NeverRetryPolicy_Tests
    {
        private NeverRetryPolicy policy;

        [SetUp]
        public void SetUp()
        {
            policy = new NeverRetryPolicy();
        }

        [Test]
        public void NeedToRetry_should_always_return_false()
        {
            policy.NeedToRetry(new List<ReplicaResult>()).Should().BeFalse();
        }
    }
}
