using System;
using JetBrains.Annotations;
using Vostok.Logging;

namespace Vostok.Clusterclient.Ordering.Weighed.Adaptive
{
    /// <summary>
    /// <para>Represents an exact replica health behaviour which uses immutable <typeparamref name="THealth"/> objects as health values.</para>
    /// <para>Implementations of all methods MUST BE thread-safe.</para>
    /// </summary>
    public interface IAdaptiveHealthImplementation<THealth>
    {
        /// <summary>
        /// Modifies replica weight using given <paramref name="health"/>.
        /// </summary>
        void ModifyWeight(THealth health, ref double weight);

        /// <summary>
        /// Returns a default health value.
        /// </summary>
        [Pure]
        THealth CreateDefaultHealth();

        /// <summary>
        /// Returns a health value obtained by increasing the <paramref name="current"/> value.
        /// </summary>
        [Pure]
        THealth IncreaseHealth(THealth current);

        /// <summary>
        /// Returns a health value obtained by decreasing the <paramref name="current"/> value.
        /// </summary>
        [Pure]
        THealth DecreaseHealth(THealth current);

        /// <summary>
        /// Returns true if provided <paramref name="x"/> and <paramref name="y"/> health values are considered equal, or false otherwise.
        /// </summary>
        [Pure]
        bool AreEqual(THealth x, THealth y);

        /// <summary>
        /// Logs the change of health for <paramref name="replica"/> from <paramref name="oldHealth"/> to <paramref name="newHealth"/>.
        /// </summary>
        void LogHealthChange(Uri replica, THealth oldHealth, THealth newHealth, ILog log);
    }
}
