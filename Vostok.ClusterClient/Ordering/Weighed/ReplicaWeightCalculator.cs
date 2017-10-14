using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;

namespace Vostok.Clusterclient.Ordering.Weighed
{
    internal class ReplicaWeightCalculator : IReplicaWeightCalculator
    {
        private readonly IList<IReplicaWeightModifier> modifiers;
        private readonly double minimumWeight;
        private readonly double maximumWeight;
        private readonly double initialWeight;

        public ReplicaWeightCalculator(
            [NotNull] IList<IReplicaWeightModifier> modifiers,
            double minimumWeight,
            double maximumWeight,
            double initialWeight)
        {
            if (modifiers == null)
                throw new ArgumentNullException(nameof(modifiers));

            if (maximumWeight < minimumWeight)
                throw new ArgumentException($"Maximum weight '{maximumWeight}' was less than minimum weight '{minimumWeight}'.");

            if (minimumWeight < 0.0)
                throw new ArgumentOutOfRangeException(nameof(minimumWeight), $"Minimum weight '{minimumWeight}' was negative.");

            if (initialWeight > maximumWeight)
                throw new ArgumentOutOfRangeException(nameof(initialWeight), $"Initial weight '{initialWeight}' was greater than maximum weight '{maximumWeight}'.");

            if (initialWeight < minimumWeight)
                throw new ArgumentOutOfRangeException(nameof(initialWeight), $"Initial weight '{initialWeight}' was less than minimum weight '{minimumWeight}'.");

            this.modifiers = modifiers;
            this.minimumWeight = minimumWeight;
            this.maximumWeight = maximumWeight;
            this.initialWeight = initialWeight;
        }

        public double GetWeight(Uri replica, IList<Uri> allReplicas, IReplicaStorageProvider storageProvider, Request request)
        {
            var weight = initialWeight;

            for (var i = 0; i < modifiers.Count; i++)
            {
                modifiers[i].Modify(replica, allReplicas, storageProvider, request, ref weight);

                if (weight < minimumWeight)
                    weight = minimumWeight;

                if (weight > maximumWeight)
                    weight = maximumWeight;
            }

            return weight;
        }
    }
}
