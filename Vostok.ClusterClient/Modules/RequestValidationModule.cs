using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Logging;

namespace Vostok.Clusterclient.Modules
{
    internal class RequestValidationModule : IRequestModule
    {
        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (!context.Request.IsValid)
            {
                LogValidationErrors(context, context.Request.Validate());
                return Task.FromResult(ClusterResult.IncorrectArguments(context.Request));
            }

            return next(context);
        }

        #region Logging

        private void LogValidationErrors(IRequestContext context, IEnumerable<string> errors)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Request is not valid:");

            foreach (var errorMessage in errors)
            {
                builder.Append("\t");
                builder.Append("--> ");
                builder.AppendLine(errorMessage);
            }

            context.Log.Error(builder.ToString());
        }

        #endregion
    }
}
