using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Model
{
    public class ClusterResult_Tests
    {
        private Request request;

        [SetUp]
        public void SetUp()
        {
            request = Request.Get("foo/bar");
        }

        [Test]
        public void Response_property_should_return_selected_response_if_provided_with_one()
        {
            var response = new Response(ResponseCode.Ok);

            var result = new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), response, request);

            result.Response.Should().BeSameAs(response);
        }

        [Test]
        public void Response_property_should_return_timeout_response_for_time_expired_status_if_not_provided_with_selected_one()
        {
            var result = new ClusterResult(ClusterResultStatus.TimeExpired, new List<ReplicaResult>(), null, request);

            result.Response.Code.Should().Be(ResponseCode.RequestTimeout);
        }

        [Test]
        public void Response_property_should_return_unknown_failure_response_for_unexpected_exception_status_if_not_provided_with_selected_one()
        {
            var result = new ClusterResult(ClusterResultStatus.UnexpectedException, new List<ReplicaResult>(), null, request);

            result.Response.Code.Should().Be(ResponseCode.UnknownFailure);
        }

        [Test]
        public void Response_property_should_return_canceled_response_for_canceled_status_if_not_provided_with_selected_one()
        {
            var result = new ClusterResult(ClusterResultStatus.Canceled, new List<ReplicaResult>(), null, request);

            result.Response.Code.Should().Be(ResponseCode.Canceled);
        }

        [Test]
        public void Throttled_factory_method_should_return_correct_result()
        {
            var result = ClusterResult.Throttled(request);

            result.Status.Should().Be(ClusterResultStatus.Throttled);
            result.Request.Should().BeSameAs(request);
            result.ReplicaResults.Should().BeEmpty();
            result.Response.Code.Should().Be(ResponseCode.TooManyRequests);
        }

        
        [TestCase(ClusterResultStatus.Success)]
        [TestCase(ClusterResultStatus.ReplicasNotFound)]
        [TestCase(ClusterResultStatus.ReplicasExhausted)]
        [TestCase(ClusterResultStatus.IncorrectArguments)]
        public void Response_property_should_return_unknown_response_for_given_status_if_not_provided_with_selected_one(ClusterResultStatus status)
        {
            var result = new ClusterResult(status, new List<ReplicaResult>(), null, request);

            result.Response.Code.Should().Be(ResponseCode.Unknown);
        }

        [Test]
        public void Replica_property_should_return_address_of_replica_which_returned_final_response()
        {
            var replicaResults = new List<ReplicaResult>
            {
                new ReplicaResult(new Uri("http://replica-1"), new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero),
                new ReplicaResult(new Uri("http://replica-2"), new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero),
                new ReplicaResult(new Uri("http://replica-3"), new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero)
            };

            var result = new ClusterResult(ClusterResultStatus.ReplicasExhausted, replicaResults, replicaResults[1].Response, request);

            result.Replica.Should().BeSameAs(replicaResults[1].Replica);
        }

        [Test]
        public void Replica_property_should_return_null_when_final_response_does_not_belong_to_any_replica_result()
        {
            var replicaResults = new List<ReplicaResult>
            {
                new ReplicaResult(new Uri("http://replica-1"), new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero),
                new ReplicaResult(new Uri("http://replica-2"), new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero),
                new ReplicaResult(new Uri("http://replica-3"), new Response(ResponseCode.ServiceUnavailable), ResponseVerdict.Reject, TimeSpan.Zero)
            };

            var result = new ClusterResult(ClusterResultStatus.ReplicasExhausted, replicaResults, new Response(ResponseCode.Ok), request);

            result.Replica.Should().BeNull();
        }

        [Test]
        public void Replica_property_should_return_null_when_there_are_no_replica_results()
        {
            var replicaResults = new List<ReplicaResult>();

            var result = new ClusterResult(ClusterResultStatus.ReplicasNotFound, replicaResults, null, request);

            result.Replica.Should().BeNull();
        }
    }
}
