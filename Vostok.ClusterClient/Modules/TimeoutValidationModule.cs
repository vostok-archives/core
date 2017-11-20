using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    internal class TimeoutValidationModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (context.Budget.Total < TimeSpan.Zero)
            {
                LogNegativeTimeout(context);
                return Task.FromResult(ClusterResult.IncorrectArguments(context.Request));
            }

            if (context.Budget.HasExpired)
            {
                LogExpiredTimeout(context);
                return Task.FromResult(ClusterResult.TimeExpired(context.Request));
            }

            return next(context);
        }

        #region Logging

        private void LogNegativeTimeout(IRequestContext context)
        {
            context.Log.Error($"Request timeout has incorrect negative value: '{context.Budget.Total}'.");
        }

        private void LogExpiredTimeout(IRequestContext context)
        {
            context.Log.Error($"Request timeout expired prematurely or just was too small. Total budget = '{context.Budget.Total.ToPrettyString()}'.");
        }

        #endregion
    }
}
