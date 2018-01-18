using System;
using NSubstitute;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Helpers
{
    internal static class Budget
    {
        public static readonly IRequestTimeBudget Infinite = WithRemaining(TimeSpan.MaxValue);
        public static readonly IRequestTimeBudget Expired = WithRemaining(TimeSpan.Zero);

        public static IRequestTimeBudget WithRemaining(TimeSpan remaining)
        {
            var budget = Substitute.For<IRequestTimeBudget>();

            budget.Total.Returns(remaining);
            budget.Remaining.Returns(remaining);
            budget.HasExpired.Returns(remaining <= TimeSpan.Zero);

            return budget;
        }
    }
}
