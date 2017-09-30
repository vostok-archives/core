using System;
using Vostok.Clusterclient.Model;

namespace Vostok.AirlockClient
{
    public class AirlockResponse
    {
        /// <summary>
        /// Is request finished without any errors.
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        /// Low level information about request.
        /// </summary>
        public ClusterResult Details { get; internal set; }

        public Exception OriginalException { get; internal set; }

        public static AirlockResponse FromClusterResult(ClusterResult result) =>
            new AirlockResponse
            {
                Details = result,
                Success = result.Status == ClusterResultStatus.Success
            };

        public static AirlockResponse Exception(Exception e) =>
            new AirlockResponse {Success = false, OriginalException = e};

        //TODO: EnsureSuccess
    }
}