using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Transforms;

namespace Vostok.Clusterclient.Modules
{
    internal class RequestTransformationModule : IRequestModule
    {
        private readonly IList<IRequestTransform> transforms;

        public RequestTransformationModule(IList<IRequestTransform> transforms)
        {
            this.transforms = transforms;
        }

        public Task<ClusterResult> ExecuteAsync(IRequestContext context, Func<IRequestContext, Task<ClusterResult>> next)
        {
            if (transforms != null && transforms.Count > 0)
            {
                foreach (var transform in transforms)
                    context.Request = transform.Transform(context.Request);
            }

            return next(context);
        }
    }
}
