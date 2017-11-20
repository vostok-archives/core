using System;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Utilities;

namespace Vostok.Clusterclient.Strategies.DelayProviders
{
    /// <summary>
    /// <para>Represents a delay provider which divides whole time budget by a fixed number (called division factor) and issues resulting value as a forking delay for all requests.</para>
    /// </summary>
    public class EqualDelaysProvider : IForkingDelaysProvider
    {
        private readonly int divisionFactor;

        public EqualDelaysProvider(int divisionFactor)
        {
            if (divisionFactor <= 0)
                throw new ArgumentOutOfRangeException(nameof(divisionFactor), "Division factor must be a positive number.");

            this.divisionFactor = divisionFactor;
        }

        public TimeSpan? GetForkingDelay(Request request, IRequestTimeBudget budget, int currentReplicaIndex, int totalReplicas)
        {
            return budget.Total.Divide(Math.Min(divisionFactor, totalReplicas));
        }

        public override string ToString()
        {
            return "equal-" + divisionFactor;
        }
    }
}
