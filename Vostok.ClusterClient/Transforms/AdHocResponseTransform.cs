using System;
using Vostok.Clusterclient.Model;

namespace Vostok.Clusterclient.Transforms
{
    /// <summary>
    /// Represents a response transform which uses external delegate to modify requests.
    /// </summary>
    public class AdHocResponseTransform : IResponseTransform
    {
        private readonly Func<Response, Response> transform;

        public AdHocResponseTransform(Func<Response, Response> transform)
        {
            this.transform = transform;
        }

        public Response Transform(Response request)
        {
            return transform(request);
        }
    }
}