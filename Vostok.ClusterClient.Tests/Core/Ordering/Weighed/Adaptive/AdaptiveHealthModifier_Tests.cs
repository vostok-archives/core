using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed.Adaptive;
using Vostok.Logging;
using Xunit;

namespace Vostok.Clusterclient.Core.Ordering.Weighed.Adaptive
{
    public class AdaptiveHealthModifier_Tests
    {
        private readonly Uri replica1;
        private readonly Uri replica2;
        private readonly IList<Uri> replicas;
        private readonly Request request;
        private readonly ConcurrentDictionary<Uri, int> storage;

        private readonly IReplicaStorageProvider storageProvider;
        private readonly IAdaptiveHealthImplementation<int> implementation;
        private readonly IAdaptiveHealthTuningPolicy tuningPolicy;
        private readonly AdaptiveHealthModifier<int> modifier;
        private readonly ILog log;

        public AdaptiveHealthModifier_Tests()
        {
            replica1 = new Uri("http://replica1");
            replica2 = new Uri("http://replica2");
            replicas = new List<Uri> {replica1, replica2};
            request = Request.Get("foo/bar");

            storageProvider = Substitute.For<IReplicaStorageProvider>();
            storageProvider.Obtain<int>(Arg.Any<string>()).Returns(storage = new ConcurrentDictionary<Uri, int>());

            implementation = Substitute.For<IAdaptiveHealthImplementation<int>>();
            tuningPolicy = Substitute.For<IAdaptiveHealthTuningPolicy>();
            modifier = new AdaptiveHealthModifier<int>(implementation, tuningPolicy, log = new ConsoleLog());
        }

        [Fact]
        public void Modify_should_not_touch_weight_when_there_is_no_health_in_storage()
        {
            var weight = 1.0;

            storage[replica2] = 123;

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            weight.Should().Be(1.0);
        }

        [Fact]
        public void Modify_should_not_call_health_implementation_when_there_is_no_health_in_storage()
        {
            var weight = 1.0;

            storage[replica2] = 123;

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            implementation.ReceivedCalls().Should().BeEmpty();
        }

        [Fact]
        public void Modify_should_call_health_implementation_when_there_is_a_health_in_storage()
        {
            var weight = 1.0;

            storage[replica1] = 123;

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            implementation.Received(1).ModifyWeight(123, ref weight);
        }

        [Fact]
        public void Modify_should_obtain_storage_with_correct_storage_key()
        {
            var weight = 1.0;

            modifier.Modify(replica1, replicas, storageProvider, request, ref weight);

            storageProvider.Received(1).Obtain<int>(implementation.GetType().FullName);
        }

        [Fact]
        public void Learn_should_obtain_storage_with_correct_storage_key()
        {
            SetupTuningAction(AdaptiveHealthAction.Increase);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storageProvider.Received(1).Obtain<int>(implementation.GetType().FullName);
        }

        [Fact]
        public void Learn_should_create_default_health_if_nothing_is_stored_yet()
        {
            SetupTuningAction(AdaptiveHealthAction.Increase);

            modifier.Learn(CreateResult(replica1), storageProvider);

            implementation.Received(1).CreateDefaultHealth();
        }

        [Fact]
        public void Learn_should_increase_health_when_selected_tuning_action_demands_it()
        {
            SetupTuningAction(AdaptiveHealthAction.Increase);

            storage[replica1] = 123;

            modifier.Learn(CreateResult(replica1), storageProvider);

            implementation.Received(1).IncreaseHealth(123);

            implementation.DidNotReceive().DecreaseHealth(Arg.Any<int>());
        }

        [Fact]
        public void Learn_should_decrease_health_when_selected_tuning_action_demands_it()
        {
            SetupTuningAction(AdaptiveHealthAction.Decrease);

            storage[replica1] = 123;

            modifier.Learn(CreateResult(replica1), storageProvider);

            implementation.Received(1).DecreaseHealth(123);

            implementation.DidNotReceive().IncreaseHealth(Arg.Any<int>());
        }

        [Fact]
        public void Learn_should_store_and_log_new_health_when_health_gets_increased_and_there_was_not_old_stored_health()
        {
            SetupTuningAction(AdaptiveHealthAction.Increase);

            implementation.CreateDefaultHealth().Returns(10);
            implementation.IncreaseHealth(10).Returns(20);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storage.Should().Contain(replica1, 20);

            implementation.Received().LogHealthChange(replica1, 10, 20, log);
        }

        [Fact]
        public void Learn_should_store_and_log_new_health_when_health_gets_increased_and_there_was_an_old_stored_health()
        {
            SetupTuningAction(AdaptiveHealthAction.Increase);

            storage[replica1] = 10;

            implementation.IncreaseHealth(10).Returns(20);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storage.Should().Contain(replica1, 20);

            implementation.Received().LogHealthChange(replica1, 10, 20, log);
        }

        [Fact]
        public void Learn_should_store_and_log_new_health_when_health_gets_decreased_and_there_was_not_old_stored_health()
        {
            SetupTuningAction(AdaptiveHealthAction.Decrease);

            implementation.CreateDefaultHealth().Returns(10);
            implementation.DecreaseHealth(10).Returns(5);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storage.Should().Contain(replica1, 5);

            implementation.Received().LogHealthChange(replica1, 10, 5, log);
        }

        [Fact]
        public void Learn_should_store_and_log_new_health_when_health_gets_decreased_and_there_was_an_old_stored_health()
        {
            SetupTuningAction(AdaptiveHealthAction.Decrease);

            storage[replica1] = 10;

            implementation.DecreaseHealth(10).Returns(5);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storage.Should().Contain(replica1, 5);

            implementation.Received().LogHealthChange(replica1, 10, 5, log);
        }

        [Fact]
        public void Learn_should_not_log_same_new_health_when_health_gets_increased_and_there_was_an_old_stored_health()
        {
            SetupTuningAction(AdaptiveHealthAction.Increase);

            storage[replica1] = 10;

            implementation.IncreaseHealth(10).Returns(11);
            implementation.AreEqual(10, 11).Returns(true);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storage.Should().Contain(replica1, 10);

            implementation.DidNotReceiveWithAnyArgs().LogHealthChange(null, 0, 0, null);
        }

        [Fact]
        public void Learn_should_not_store_and_log_same_new_health_when_health_gets_decreased_and_there_was_an_old_stored_health()
        {
            SetupTuningAction(AdaptiveHealthAction.Decrease);

            storage[replica1] = 10;

            implementation.DecreaseHealth(10).Returns(9);
            implementation.AreEqual(10, 9).Returns(true);

            modifier.Learn(CreateResult(replica1), storageProvider);

            storage.Should().Contain(replica1, 10);

            implementation.DidNotReceiveWithAnyArgs().LogHealthChange(null, 0, 0, null);
        }

        private static ReplicaResult CreateResult(Uri replica)
        {
            return new ReplicaResult(replica, Responses.Timeout, ResponseVerdict.DontKnow, TimeSpan.Zero);
        }

        private void SetupTuningAction(AdaptiveHealthAction action)
        {
            tuningPolicy.SelectAction(Arg.Any<ReplicaResult>()).Returns(action);
        }
    }
}
