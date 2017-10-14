using System;
using System.Collections.Generic;
using Vostok.Logging;

namespace Vostok.Clusterclient.Ordering.Weighed
{
    internal class WeighedReplicaOrderingBuilder : IWeighedReplicaOrderingBuilder
    {
        private readonly List<IReplicaWeightModifier> modifiers;

        public WeighedReplicaOrderingBuilder(ILog log)
        {
            Log = log;
            MinimumWeight = ClusterClientDefaults.MinimumReplicaWeight;
            MaximumWeight = ClusterClientDefaults.MaximumReplicaWeight;
            InitialWeight = ClusterClientDefaults.InitialReplicaWeight;

            modifiers = new List<IReplicaWeightModifier>();
        }

        public ILog Log { get; }

        public double MinimumWeight { get; set; }

        public double MaximumWeight { get; set; }

        public double InitialWeight { get; set; }

        public WeighedReplicaOrdering Build()
        {
            return new WeighedReplicaOrdering(modifiers, MinimumWeight, MaximumWeight, InitialWeight);
        }

        public void AddModifier(IReplicaWeightModifier modifier)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));

            modifiers.Add(modifier);
        }
    }
}
