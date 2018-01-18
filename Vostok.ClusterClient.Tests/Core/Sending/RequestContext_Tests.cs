using System;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Modules;
using Vostok.Clusterclient.Strategies;
using Vostok.ClusterClient.Tests.Helpers;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Sending
{
    public class RequestContext_Tests
    {
        private RequestContext context;

        [SetUp]
        public void SetUp()
        {
            var log = new ConsoleLog();

            context = new RequestContext(Request.Get("foo/bar"), Strategy.Sequential2, Budget.Infinite, log, CancellationToken.None, null, int.MaxValue);
        }

        [Test]
        public void SetReplicaResult_should_add_result_to_list_if_there_are_no_results_with_same_replica()
        {
            var result1 = CreateResult("replica1");
            var result2 = CreateResult("replica2");
            var result3 = CreateResult("replica3");

            context.SetReplicaResult(result1);
            context.SetReplicaResult(result2);
            context.SetReplicaResult(result3);

            context.FreezeReplicaResults().Should().Equal(result1, result2, result3);
        }

        [Test]
        public void SetReplicaResult_should_overwrite_existing_result_with_equal_replica()
        {
            var result1 = CreateResult("replica1");
            var result2 = CreateResult("replica2");
            var result3 = CreateResult("replica3");
            var result4 = CreateResult("replica2");
            var result5 = CreateResult("replica1");

            context.SetReplicaResult(result1);
            context.SetReplicaResult(result2);
            context.SetReplicaResult(result3);
            context.SetReplicaResult(result4);
            context.SetReplicaResult(result5);

            context.FreezeReplicaResults().Should().Equal(result5, result4, result3);
        }

        [Test]
        public void SetReplicaResult_should_not_affect_already_frozen_results_list()
        {
            var result1 = CreateResult("replica1");
            var result2 = CreateResult("replica2");
            var result3 = CreateResult("replica3");

            context.SetReplicaResult(result1);
            context.SetReplicaResult(result2);

            var results = context.FreezeReplicaResults();

            context.SetReplicaResult(result3);

            results.Should().Equal(result1, result2);
        }

        [Test]
        public void ResetReplicaResults_should_initialize_new_results_list_and_allow_to_add_new_results()
        {
            var result1 = CreateResult("replica1");
            var result2 = CreateResult("replica2");
            var result3 = CreateResult("replica3");
            var result4 = CreateResult("replica4");
            var result5 = CreateResult("replica5");

            context.SetReplicaResult(result1);
            context.SetReplicaResult(result2);
            context.SetReplicaResult(result3);

            context.FreezeReplicaResults().Should().Equal(result1, result2, result3);
            context.ResetReplicaResults();

            context.SetReplicaResult(result4);
            context.SetReplicaResult(result5);

            context.FreezeReplicaResults().Should().Equal(result4, result5);
        }

        private static ReplicaResult CreateResult(string replica)
        {
            return new ReplicaResult(new Uri("http://" + replica), Responses.Timeout, ResponseVerdict.Accept, TimeSpan.Zero);
        }
    }
}
