using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Misc;
using Vostok.Clusterclient.Model;

namespace Vostok.ClusterClient.Tests.Core.Misc
{
    public class LastAcceptedResponseSelector_Tests
    {
        private LastAcceptedResponseSelector selector;
        private List<ReplicaResult> results;

        [SetUp]
        public void Setup()
        {
            selector = new LastAcceptedResponseSelector();
            results = new List<ReplicaResult>();
        }

        [Test]
        public void Should_return_null_when_there_are_no_results()
        {
            selector.Select(results).Should().BeNull();
        }

        [Test]
        public void Should_return_last_of_the_accepted_responses_if_there_are_any()
        {
            results.Add(CreateResult(ResponseVerdict.Accept));
            results.Add(CreateResult(ResponseVerdict.Reject));
            results.Add(CreateResult(ResponseVerdict.Reject));
            results.Add(CreateResult(ResponseVerdict.Accept));
            results.Add(CreateResult(ResponseVerdict.Reject));

            selector.Select(results).Should().BeSameAs(results[3].Response);
        }

        [Test]
        public void Should_return_last_of_the_known_responses_if_there_are_no_accepted_ones()
        {
            results.Add(CreateResult(ResponseVerdict.Reject, ResponseCode.Unknown));
            results.Add(CreateResult(ResponseVerdict.Reject, ResponseCode.Unknown));
            results.Add(CreateResult(ResponseVerdict.Reject));
            results.Add(CreateResult(ResponseVerdict.Reject));
            results.Add(CreateResult(ResponseVerdict.Reject, ResponseCode.Unknown));

            selector.Select(results).Should().BeSameAs(results[3].Response);
        }

        [Test]
        public void Should_return_just_the_last_response_if_there_are_no_accepted_or_known_ones()
        {
            results.Add(CreateResult(ResponseVerdict.Reject, ResponseCode.Unknown));
            results.Add(CreateResult(ResponseVerdict.Reject, ResponseCode.Unknown));
            results.Add(CreateResult(ResponseVerdict.Reject, ResponseCode.Unknown));

            selector.Select(results).Should().BeSameAs(results.Last().Response);
        }

        private static ReplicaResult CreateResult(ResponseVerdict verdict, ResponseCode code = ResponseCode.Ok)
        {
            return new ReplicaResult(new Uri("http://host:123/"), new Response(code), verdict, TimeSpan.Zero);
        }
    }
}
