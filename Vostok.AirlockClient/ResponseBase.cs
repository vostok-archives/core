using Vostok.Clusterclient.Model;

namespace Vostok.AirlockClient
{
    public abstract class ResponseBase
    {
        /// <summary>
        /// Is request finished without any errors.
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        /// Low level information about request.
        /// </summary>
        public ClusterResult Details { get; internal set; }

        //TODO: EnsureSuccess
    }
}