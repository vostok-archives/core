using System;
using System.Collections.Generic;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    /// <summary>
    /// <para>Represents a weight modifier which uses a concept of replica health that dynamically increases and decreases in response to replica behaviour.</para>
    /// <para>The exact nature of health and its application to weight is defined by an <see cref="IAdaptiveHealthImplementation{THealth}"/> instance.</para>
    /// <para>The actions to be taken on replica health in response to observed <see cref="ReplicaResult"/>s are defined by <see cref="IAdaptiveHealthTuningPolicy"/> instance.</para>
    /// </summary>
    /// <typeparam name="THealth">Type of health values used in <see cref="IAdaptiveHealthImplementation{THealth}"/>.</typeparam>
    public class AdaptiveHealthModifier<THealth> : IReplicaWeightModifier
    {
        private readonly IAdaptiveHealthImplementation<THealth> implementation;
        private readonly IAdaptiveHealthTuningPolicy tuningPolicy;
        private readonly ILog log;
        private readonly string storageKey;

        public AdaptiveHealthModifier(IAdaptiveHealthImplementation<THealth> implementation, IAdaptiveHealthTuningPolicy tuningPolicy, ILog log)
        {
            this.implementation = implementation;
            this.tuningPolicy = tuningPolicy;
            this.log = log ?? new SilentLog();

            storageKey = implementation.GetType().FullName;
        }

        public void Modify(Uri replica, IList<Uri> allReplicas, IReplicaStorageProvider storageProvider, Request request, ref double weight)
        {
            THealth currentHealth;

            if (storageProvider.Obtain<THealth>(storageKey).TryGetValue(replica, out currentHealth))
                implementation.ModifyWeight(currentHealth, ref weight);
        }

        public void Learn(ReplicaResult result, IReplicaStorageProvider storageProvider)
        {
            var storage = storageProvider.Obtain<THealth>(storageKey);

            while (true)
            {
                THealth currentHealth;
                THealth newHealth;
                bool foundHealth;

                if (!(foundHealth = storage.TryGetValue(result.Replica, out currentHealth)))
                    currentHealth = implementation.CreateDefaultHealth();

                switch (tuningPolicy.SelectAction(result))
                {
                    case AdaptiveHealthAction.Increase:
                        newHealth = implementation.IncreaseHealth(currentHealth);
                        break;

                    case AdaptiveHealthAction.Decrease:
                        newHealth = implementation.DecreaseHealth(currentHealth);
                        break;

                    default:
                        newHealth = currentHealth;
                        break;
                }

                if (implementation.AreEqual(currentHealth, newHealth))
                    break;

                var updatedHealth = foundHealth
                    ? storage.TryUpdate(result.Replica, newHealth, currentHealth)
                    : storage.TryAdd(result.Replica, newHealth);

                if (updatedHealth)
                {
                    implementation.LogHealthChange(result.Replica, currentHealth, newHealth, log);
                    break;
                }
            }
        }
    }
}
