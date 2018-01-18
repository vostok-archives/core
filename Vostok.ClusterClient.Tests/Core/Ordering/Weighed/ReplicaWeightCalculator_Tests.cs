using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed;
using Vostok.ClusterClient.Tests.Helpers;

namespace Vostok.ClusterClient.Tests.Core.Ordering.Weighed
{
    public class ReplicaWeightCalculator_Tests
    {
        private const double minWeight = 0.0;
        private const double maxWeight = 10.0;
        private const double initialWeight = 1.0;

        private Uri replica;
        private IList<Uri> replicas;
        private Request request;
        private IReplicaStorageProvider storageProvider;
        private List<IReplicaWeightModifier> modifiers;
        private ReplicaWeightCalculator calculator;

        [SetUp]
        public void SetUp()
        {
            replica = new Uri("http://replica");
            replicas = new List<Uri> {replica};
            request = Request.Get("foo/bar");
            modifiers = new List<IReplicaWeightModifier>();
            storageProvider = Substitute.For<IReplicaStorageProvider>();
            calculator = new ReplicaWeightCalculator(modifiers, minWeight, maxWeight, initialWeight);
        }

        [Test]
        public void Ctor_should_throw_an_error_when_modifiers_list_is_null()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Action action = () => new ReplicaWeightCalculator(null, minWeight, maxWeight, initialWeight);

            action.ShouldThrow<ArgumentNullException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_minimum_weight_is_negative()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, -0.01, maxWeight, initialWeight);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_minimum_weight_is_greater_than_maximum_weight()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, maxWeight, minWeight, initialWeight);

            action.ShouldThrow<ArgumentException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_initial_weight_is_greater_than_maximum_weight()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, minWeight, maxWeight, maxWeight + 1);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_initial_weight_is_less_than_minimum_weight()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, minWeight, maxWeight, minWeight - 1);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void GetWeight_should_return_initial_weight_when_there_are_no_modifiers()
        {
            calculator.GetWeight(replica, replicas, storageProvider, request).Should().Be(initialWeight);
        }

        [Test]
        public void GetWeight_should_call_all_weight_modifiers_in_order()
        {
            modifiers.Add(CreateModifier(w => w + 1));
            modifiers.Add(CreateModifier(w => w*2));
            modifiers.Add(CreateModifier(w => w + 3));

            calculator.GetWeight(replica, replicas, storageProvider, request).Should().Be(7.0);

            Received.InOrder(
                () =>
                {
                    var w1 = initialWeight;
                    var w2 = w1 + 1;
                    var w3 = w2*2;

                    modifiers[0].Modify(replica, replicas, storageProvider, request, ref w1);
                    modifiers[1].Modify(replica, replicas, storageProvider, request, ref w2);
                    modifiers[2].Modify(replica, replicas, storageProvider, request, ref w3);
                });
        }

        [Test]
        public void GetWeight_should_enforce_weight_limits_between_all_modifier_invocations()
        {
            modifiers.Add(CreateModifier(w => 100.0));
            modifiers.Add(CreateModifier(w => -100.0));
            modifiers.Add(CreateModifier(w => w + 2));

            calculator.GetWeight(replica, replicas, storageProvider, request).Should().Be(2.0);

            Received.InOrder(
                () =>
                {
                    var w1 = initialWeight;
                    var w2 = maxWeight;
                    var w3 = minWeight;

                    modifiers[0].Modify(replica, replicas, storageProvider, request, ref w1);
                    modifiers[1].Modify(replica, replicas, storageProvider, request, ref w2);
                    modifiers[2].Modify(replica, replicas, storageProvider, request, ref w3);
                });
        }

        private static IReplicaWeightModifier CreateModifier(Func<double, double> transform)
        {
            var modifier = Substitute.For<IReplicaWeightModifier>();

            var dummy = 0.0;

            modifier
                // ReSharper disable AssignNullToNotNullAttribute
                .WhenForAnyArgs(m => m.Modify(null, null, null, null, ref dummy))
                // ReSharper restore AssignNullToNotNullAttribute
                .Do(info => { info[4] = transform(info.Arg<double>()); });

            return modifier;
        }
    }
}
