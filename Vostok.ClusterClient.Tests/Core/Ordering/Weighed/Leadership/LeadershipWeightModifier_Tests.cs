using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed.Leadership;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Weighed.Leadership
{
    public class LeadershipWeightModifier_Tests
    {
        private Uri replica;
        private IList<Uri> replicas;
        private Request request;
        private Response response;
        private ReplicaResult result;
                         
        private double weight;

        private ILeaderResultDetector resultDetector;
        private IReplicaStorageProvider storageProvider;
        private ConcurrentDictionary<Uri, bool> storage;
        private LeadershipWeightModifier modifier;

        [SetUp]
        public void SetUp()
        {
            replica = new Uri("http://replica");
            replicas = new List<Uri> {replica};
            request = Request.Get("foo/bar");
            response = new Response(ResponseCode.Ok);
            result = new ReplicaResult(replica, response, ResponseVerdict.Accept, TimeSpan.Zero);
            weight = 1.0;

            resultDetector = Substitute.For<ILeaderResultDetector>();
            storageProvider = Substitute.For<IReplicaStorageProvider>();
            storageProvider.Obtain<bool>(Arg.Any<string>()).Returns(storage = new ConcurrentDictionary<Uri, bool>());

            modifier = new LeadershipWeightModifier(resultDetector, new ConsoleLog());
        }

        [Test]
        public void Modify_should_zero_out_the_weight_if_there_is_no_info_stored_about_replica()
        {
            modifier.Modify(replica, replicas, storageProvider, request, ref weight);

            weight.Should().Be(0.0);
        }

        [Test]
        public void Modify_should_zero_out_the_weight_if_replica_is_not_a_leader()
        {
            storage[replica] = false;

            modifier.Modify(replica, replicas, storageProvider, request, ref weight);

            weight.Should().Be(0.0);
        }

        [Test]
        public void Modify_should_not_touch_the_weight_if_replica_is_a_leader()
        {
            storage[replica] = true;

            modifier.Modify(replica, replicas, storageProvider, request, ref weight);

            weight.Should().Be(1.0);
        }

        
        [TestCase(null, true, true)]
        [TestCase(null, false, null)]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        public void Learn_should_correctly_store_new_health(bool? old, bool isLeader, bool? expected)
        {
            if (old.HasValue)
                storage[replica] = old.Value;

            resultDetector.IsLeaderResult(result).Returns(isLeader);

            modifier.Learn(result, storageProvider);

            if (expected.HasValue)
            {
                storage[replica].Should().Be(expected.Value);
            }
            else
            {
                storage.Should().NotContainKey(replica);
            }
        }
    }
}
