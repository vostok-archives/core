using JetBrains.Annotations;
using Vostok.Logging;

namespace Vostok.Clusterclient.Ordering.Weighed
{
    /// <summary>
    /// Represents a builder used to construct a <see cref="WeighedReplicaOrdering"/> instance.
    /// </summary>
    public interface IWeighedReplicaOrderingBuilder
    {
        ILog Log { get; }

        /// <summary>
        /// Gets or sets the minimum possible replica weight. It must be greater or equal to zero.
        /// </summary>
        double MinimumWeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum possible replica weight. It must be greater or equal to <see cref="MinimumWeight"/>.
        /// </summary>
        double MaximumWeight { get; set; }

        /// <summary>
        /// Gets or sets the initial (default) replica weight. It must be between <see cref="MinimumWeight"/> and <see cref="MaximumWeight"/>.
        /// </summary>
        double InitialWeight { get; set; }

        /// <summary>
        /// Adds given <paramref name="modifier"/> to the modifiers chain.
        /// </summary>
        void AddModifier([NotNull] IReplicaWeightModifier modifier);
    }
}
