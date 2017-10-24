using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Sending;

namespace Vostok.Clusterclient.Modules
{
    internal static class RequestModuleChainBuilder
    {
        public static IList<IRequestModule> BuildChain(IClusterClientConfiguration config, IReplicaStorageProvider storageProvider)
        {
            var responseClassifier = new ResponseClassifier();
            var requestConverter = new RequestConverter(config.Log);
            var requestSender = new RequestSender(config, storageProvider, responseClassifier, requestConverter, config.Transport);
            var resultStatusSelector = new ClusterResultStatusSelector();

            var modules = new List<IRequestModule>(15 + config.Modules?.Count ?? 0)
            {
                new ErrorCatchingModule(),
                new RequestTransformationModule(config.RequestTransforms),
                new OperationNameFallbackModule(),
                new RequestPriorityApplicationModule()
            };

            if (config.Modules != null)
                modules.AddRange(config.Modules);

            if (config.EnableTracing)
                modules.Add(new TracingModule(config.ServiceName));

            modules.Add(new LoggingModule(config.LogRequestDetails, config.LogResultDetails));
            modules.Add(new ResponseTransformationModule(config.ResponseTransforms));
            modules.Add(new ErrorCatchingModule());
            modules.Add(new RequestValidationModule());
            modules.Add(new TimeoutValidationModule());
            modules.Add(new RequestRetryModule(config.RetryPolicy, config.RetryStrategy));

            if (config.AdaptiveThrottling != null)
                modules.Add(new AdaptiveThrottlingModule(config.AdaptiveThrottling));

            if (config.ReplicaBudgeting != null)
                modules.Add(new ReplicaBudgetingModule(config.ReplicaBudgeting));

            modules.Add(new AbsoluteUrlSenderModule(config.Transport, responseClassifier, config.ResponseCriteria, resultStatusSelector));
            modules.Add(new RequestExecutionModule(config.ClusterProvider, config.ReplicaOrdering, config.ResponseSelector, 
                storageProvider, requestSender, resultStatusSelector));

            return modules;
        }

        public static Func<IRequestContext, Task<ClusterResult>> BuildChainDelegate(IList<IRequestModule> modules)
        {
            Func<IRequestContext, Task<ClusterResult>> result = ctx => throw new NotSupportedException();

            for (var i = modules.Count - 1; i >= 0; i--)
            {
                var currentModule = modules[i];
                var currentResult = result;

                result = ctx =>
                {
                    if (ctx.CancellationToken.IsCancellationRequested)
                        return Task.FromResult(ClusterResult.Canceled(ctx.Request));

                    return currentModule.ExecuteAsync(ctx, currentResult);
                };
            }

            return result;
        }
    }
}
