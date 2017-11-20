using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Clusterclient.Criteria;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Ordering.Storage;
using Vostok.Clusterclient.Transport;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Clusterclient.Sending
{
    internal class RequestSender : IRequestSender
    {
        private readonly IClusterClientConfiguration configuration;
        private readonly IReplicaStorageProvider storageProvider;
        private readonly IResponseClassifier responseClassifier;
        private readonly IRequestConverter requestConverter;
        private readonly ITransport transport;

        public RequestSender(
            IClusterClientConfiguration configuration,
            IReplicaStorageProvider storageProvider,
            IResponseClassifier responseClassifier,
            IRequestConverter requestConverter,
            ITransport transport)
        {
            this.configuration = configuration;
            this.storageProvider = storageProvider;
            this.responseClassifier = responseClassifier;
            this.requestConverter = requestConverter;
            this.transport = transport;
        }

        public async Task<ReplicaResult> SendToReplicaAsync(Uri replica, Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (configuration.LogReplicaRequests)
                LogRequest(replica, timeout);

            var watch = Stopwatch.StartNew();

            var absoluteRequest = requestConverter.TryConvertToAbsolute(request, replica);

            var response = await SendRequestAsync(absoluteRequest, timeout, cancellationToken).ConfigureAwait(false);

            var responseVerdict = responseClassifier.Decide(response, configuration.ResponseCriteria);

            var result = new ReplicaResult(replica, response, responseVerdict, watch.Elapsed);

            if (configuration.LogReplicaResults)
                LogResult(result);

            configuration.ReplicaOrdering.Learn(result, storageProvider);

            return result;
        }

        private async Task<Response> SendRequestAsync([CanBeNull] Request request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (request == null)
                return Responses.Unknown;

            try
            {
                var response = await transport.SendAsync(request, timeout, cancellationToken).ConfigureAwait(false);
                if (response.Code == ResponseCode.Canceled)
                    throw new OperationCanceledException();

                return response;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception error)
            {
                LogTransportException(error);
                return Responses.UnknownFailure;
            }
        }

        #region Logging

        private void LogRequest(Uri replica, TimeSpan timeout)
        {
            configuration.Log.Info($"Sending request to replica '{replica}' with timeout {timeout.ToPrettyString()}.");
        }

        private void LogResult(ReplicaResult result)
        {
            configuration.Log.Info($"Result: replica = '{result.Replica}'; code = {(int) result.Response.Code} ('{result.Response.Code}'); verdict = {result.Verdict}; time = {result.Time.ToPrettyString()}.");
        }

        private void LogTransportException(Exception error)
        {
            configuration.Log.Error("Transport implementation threw an exception.", error);
        }

        #endregion
    }
}
