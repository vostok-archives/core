using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed.Gray;
using Vostok.Logging.Logs;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Weighed.Gray
{
    public class GrayListModifier_Tests
    {
        private double weight;
        private Uri replica1;
        private Uri replica2;
        private IList<Uri> replicas;
        private Request request;
        private DateTime currentTime;
        private ConcurrentDictionary<Uri, DateTime> storage;

        private IReplicaStorageProvider storageProvider;
        private IGrayPeriodProvider periodProvider;
        private ITimeProvider timeProvider;

        private GrayListModifier modifier;

        [SetUp]
        public void SetUp()
        {
            weight = 1.0;
            request = Request.Get("foo/bar");
            replica1 = new Uri("http://replica1");
            replica2 = new Uri("http://replica2");
            replicas = new List<Uri> {replica1, replica2};
            storage = new ConcurrentDictionary<Uri, DateTime>();

            storageProvider = Substitute.For<IReplicaStorageProvider>();
            storageProvider.Obtain<DateTime>(Arg.Any<string>()).Returns(storage);

            periodProvider = Substitute.For<IGrayPeriodProvider>();
            periodProvider.GetGrayPeriod().Returns(5.Minutes());

            timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrentTime().Returns(currentTime = DateTime.UtcNow);

            modifier = new GrayListModifier(periodProvider, timeProvider, new ConsoleLog());
        }

        [Test]
        public void Learn_method_should_do_nothing_when_response_verdict_is_accept()
        {
            modifier.Learn(CreateResult(replica1, ResponseVerdict.Accept), storageProvider);

            storage.Should().BeEmpty();
        }

        [Test]
        public void Learn_method_should_do_nothing_when_response_verdict_is_dont_know()
        {
            modifier.Learn(CreateResult(replica1, ResponseVerdict.DontKnow), storageProvider);

            storage.Should().BeEmpty();
        }

        [Test]
        public void Learn_method_should_put_replica_with_rejected_response_to_gray_list()
        {
            modifier.Learn(CreateResult(replica1, ResponseVerdict.Reject), storageProvider);

            storage.Should().Contain(replica1, currentTime);
        }

        [Test]
        public void Learn_method_should_refresh_gray_timestamp_if_a_previous_timestamp_is_already_stored_for_replica()
        {
            modifier.Learn(CreateResult(replica1, ResponseVerdict.Reject), storageProvider);

            ShiftCurrentTime(1.Minutes());

            modifier.Learn(CreateResult(replica1, ResponseVerdict.Reject), storageProvider);

            storage.Should().Contain(replica1, currentTime);
        }

        [Test]
        public void Modify_method_should_not_modify_weight_if_nothing_is_stored_for_given_replica()
        {
            storage[replica1] = currentTime;

            modifier.Modify(replica2, replicas, storageProvider, request, ref weight);

            weight.Should().Be(1.0);
        }

        [Test]
        public void Modify_method_should_not_modify_weight_if_a_stale_timestamp_is_stored_for_given_replica()
        {
            storage[replica1] = currentTime - 5.Minutes() - 1.Seconds();

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            weight.Should().Be(1.0);
        }

        [Test]
        public void Modify_method_should_remove_stale_timestamps()
        {
            storage[replica1] = currentTime - 5.Minutes() - 1.Seconds();

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            storage.Should().NotContainKey(replica1);
        }

        [Test]
        public void Modify_method_should_turn_weight_to_zero_for_recently_grayed_replicas()
        {
            storage[replica1] = currentTime - 4.Minutes();

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            weight.Should().Be(0.0);
        }

        [Test]
        public void Modify_method_should_keep_recent_gray_timestamps()
        {
            storage[replica1] = currentTime - 4.Minutes();

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            storage.Should().ContainKey(replica1);
        }

        private void ShiftCurrentTime(TimeSpan delta)
        {
            timeProvider.GetCurrentTime().Returns(currentTime = currentTime + delta);
        }

        private ReplicaResult CreateResult(Uri replica, ResponseVerdict verdict)
        {
            return new ReplicaResult(replica, Responses.Timeout, verdict, TimeSpan.Zero);
        }
    }
}
