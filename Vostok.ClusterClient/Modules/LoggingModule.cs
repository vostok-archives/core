using System;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    internal class LoggingModule : IRequestModule
    {
        private readonly bool logRequests;
        private readonly bool logResults;

        public LoggingModule(bool logRequests, bool logResults)
        {
            this.logRequests = logRequests;
            this.logResults = logResults;
        }

        public async Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (logRequests)
            {
                LogRequestDetails(context);
            }

            var result = await next(context).ConfigureAwait(false);

            if (logResults)
            {
                if (result.Status == ClusterResultStatus.Success)
                {
                    LogSuccessfulResult(context, result);
                }
                else
                {
                    LogFailedResult(context, result);
                }
            }

            return result;
        }

        #region Logging

        private static void LogRequestDetails(IRequestContext context)
        {
            context.Log.Info($"Sending request '{context.Request.ToString(false, false)}'. Timeout = {context.Budget.Total.ToPrettyString()}. Strategy = '{context.Strategy}'.");
        }

        private static void LogSuccessfulResult(IRequestContext context, ClusterResult result)
        {
            context.Log.Info($"Success. Response code = {(int) result.Response.Code} ('{result.Response.Code}'). Time = {context.Budget.Elapsed.ToPrettyString()}.");
        }

        private static void LogFailedResult(IRequestContext context, ClusterResult result)
        {
            var message = $"Failed with status '{result.Status}'. Response code = {(int) result.Response.Code} ('{result.Response.Code}'). Time = {context.Budget.Elapsed.ToPrettyString()}.";

            if (result.Status == ClusterResultStatus.Canceled)
            {
                context.Log.Warn(message);
            }
            else
            {
                context.Log.Error(message);
            }
        }

        #endregion
    }
}
