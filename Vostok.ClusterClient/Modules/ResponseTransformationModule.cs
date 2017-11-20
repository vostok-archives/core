using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transforms;

namespace Vostok.Clusterclient.Modules
{
    internal class ResponseTransformationModule : IRequestModule
    {
        private readonly IList<IResponseTransform> transforms;

        public ResponseTransformationModule(IList<IResponseTransform> transforms)
        {
            this.transforms = transforms;
        }

        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (transforms != null && transforms.Count > 0)
            {
                return TransformInternal(context, next);
            }

            return next(context);
        }

        private async Task<ClusterResult> TransformInternal(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            var result = await next(context).ConfigureAwait(false);

            var response = result.Response;

            foreach (var transform in transforms)
            {
                response = transform.Transform(response);
            }

            if (!ReferenceEquals(response, result.Response))
            {
                result = new ClusterResult(result.Status, result.ReplicaResults, response, result.Request);
            }

            return result;
        }
    }
}
