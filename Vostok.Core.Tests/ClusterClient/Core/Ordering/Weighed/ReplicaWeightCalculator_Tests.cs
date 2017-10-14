using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Vostok.Clusterclient.Helpers;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Ordering.Weighed;
using NUnit.Framework;

namespace Vostok.Clusterclient.Core.Ordering.Weighed
{
    public class ReplicaWeightCalculator_Tests
    {
        private const double MinWeight = 0.0;
        private const double MaxWeight = 10.0;
        private const double InitialWeight = 1.0;

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
            calculator = new ReplicaWeightCalculator(modifiers, MinWeight, MaxWeight, InitialWeight);
        }

        [Test]
        public void Ctor_should_throw_an_error_when_modifiers_list_is_null()
        {
            Action action = () => new ReplicaWeightCalculator(null, MinWeight, MaxWeight, InitialWeight);

            action.ShouldThrow<ArgumentNullException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_minimum_weight_is_negative()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, -0.01, MaxWeight, InitialWeight);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_minimum_weight_is_greater_than_maximum_weight()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, MaxWeight, MinWeight, InitialWeight);

            action.ShouldThrow<ArgumentException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_initial_weight_is_greater_than_maximum_weight()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, MinWeight, MaxWeight, MaxWeight + 1);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Ctor_should_throw_an_error_when_initial_weight_is_less_than_minimum_weight()
        {
            Action action = () => new ReplicaWeightCalculator(modifiers, MinWeight, MaxWeight, MinWeight - 1);

            action.ShouldThrow<ArgumentOutOfRangeException>().Which.ShouldBePrinted();
        }

        [Test]
        public void GetWeight_should_return_initial_weight_when_there_are_no_modifiers()
        {
            calculator.GetWeight(replica, replicas, storageProvider, request).Should().Be(InitialWeight);
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
                    var w1 = InitialWeight;
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
                    var w1 = InitialWeight;
                    var w2 = MaxWeight;
                    var w3 = MinWeight;

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
                .WhenForAnyArgs(m => m.Modify(null, null, null, null, ref dummy))
                .Do(info => { info[4] = transform(info.Arg<double>()); });

            return modifier;
        }
    }
}
