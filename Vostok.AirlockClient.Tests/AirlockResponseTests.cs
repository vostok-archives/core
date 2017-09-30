using System;
using System.Collections.Generic;
using Shouldly;
using Vostok.Clusterclient.Model;
using Xunit;

namespace Vostok.AirlockClient.Tests
{
    public class AirlockResponseTests
    {
        [Fact]
        public void EnsureSuccess_should_throw_exception_if_success_false()
        {
            var error = new ClusterResult(ClusterResultStatus.Canceled, new List<ReplicaResult>(), null, Request.Get("foo"));
            var sut = AirlockResponse.FromClusterResult(error);

            var actualException = Should.Throw<AirlockRequestFailException>(() => sut.EnsureSuccess());
            actualException.Response.ShouldBe(sut);
        }

        [Fact]
        public void Success_should_be_false_if_bad_ClusterResult()
        {
            var error = new ClusterResult(ClusterResultStatus.Canceled, new List<ReplicaResult>(), null, Request.Get("foo"));
            var sut = AirlockResponse.FromClusterResult(error);

            var actual = sut.Success;

            actual.ShouldBe(false);
        }

        [Fact]
        public void Success_should_be_false_if_exception()
        {
            var error = new Exception();
            var sut = AirlockResponse.Exception(error);

            var actual = sut.Success;

            actual.ShouldBe(false);
        }

        [Fact]
        public void Success_should_be_true_if_good_ClusterResult()
        {
            var error = new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), null, Request.Get("foo"));
            var sut = AirlockResponse.FromClusterResult(error);

            var actual = sut.Success;

            actual.ShouldBe(true);
        }
    }
}
